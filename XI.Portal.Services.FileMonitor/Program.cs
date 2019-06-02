using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Serilog;
using Serilog.Events;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.Configuration.Providers;
using XI.Portal.Library.Logging;

namespace XI.Portal.Services.FileMonitor
{
    internal class Program
    {
        private static void Main()
        {
            var container = new UnityContainer();

            var logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.ColoredConsole()
                .CreateLogger();
            Log.Logger = logger;

            container.RegisterType<AppSettingConfigurationProvider>();
            container.RegisterType<AwsSecretConfigurationProvider>();
            container.RegisterType<AwsConfiguration>();
            container.RegisterType<DatabaseConfiguration>();

            container.RegisterType<ILogger>(new ContainerControlledLifetimeManager(), new InjectionFactory((ctr, type, name) => logger));
            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();

            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UseSerilog();

                x.Service<FileMonitorService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                    s.WhenShutdown(service => service.Shutdown());
                });

                x.RunAsLocalSystem();
                x.UseAssemblyInfoForServiceInfo();
            });
        }
    }

    internal class FileMonitorService
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly ILogger logger;

        private readonly Dictionary<Guid, Thread> workerThreads;
        private Thread controllingThread;

        private CancellationTokenSource cts;

        public FileMonitorService(ILogger logger, IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));

            workerThreads = new Dictionary<Guid, Thread>();
        }

        public void Start()
        {
            logger.Information("Starting new File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Starting new File Monitor instance");

            cts = new CancellationTokenSource();
            controllingThread = new Thread(() => StartControllingThread(cts.Token));
            controllingThread.Start();
        }

        public void Stop()
        {
            logger.Information("Stopping current File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Stopping current File Monitor instance");

            cts?.Cancel();

            foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
        }

        public void Shutdown()
        {
            logger.Information("Shutting down current FileMonitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Shutting down current FileMonitor instance");

            cts?.Cancel();

            foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
        }

        private void StartControllingThread(CancellationToken token)
        {
            databaseLogger.CreateSystemLogAsync("Info", "File Monitor controlling thread started");

            var lastLoop = DateTime.MinValue;
            while (!token.IsCancellationRequested)
            {
                if (DateTime.UtcNow < lastLoop.AddSeconds(30))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                using (var context = contextProvider.GetContext())
                {
                    var fileMonitors = context.FileMonitors.Include(monitor => monitor.GameServer).ToList();

                    foreach (var fileMonitor in fileMonitors)
                        if (workerThreads.ContainsKey(fileMonitor.FileMonitorId))
                        {
                            var workerThread = workerThreads[fileMonitor.FileMonitorId];

                            if (workerThread != null && workerThread.IsAlive) continue;
                            var thread = new Thread(() =>
                                MonitorLog(cts.Token, fileMonitor.FileMonitorId));
                            thread.Start();

                            databaseLogger.CreateSystemLogAsync("Info",
                                $"File Monitor created worker thread for {fileMonitor.FileMonitorId}");

                            workerThreads[fileMonitor.FileMonitorId] = thread;
                        }
                        else
                        {
                            var thread = new Thread(() =>
                                MonitorLog(cts.Token, fileMonitor.FileMonitorId));
                            thread.Start();

                            databaseLogger.CreateSystemLogAsync("Info",
                                $"File Monitor created worker thread for {fileMonitor.FileMonitorId}");

                            workerThreads.Add(fileMonitor.FileMonitorId, thread);
                        }
                }

                lastLoop = DateTime.UtcNow;
            }
        }

        private void MonitorLog(CancellationToken token, Guid fileMonitorId)
        {
            using (var context = contextProvider.GetContext())
            {
                try
                {
                    var lastLoop = DateTime.MinValue;
                    while (!token.IsCancellationRequested)
                    {
                        if (DateTime.UtcNow < lastLoop.AddSeconds(30))
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        var fileMonitor = context.FileMonitors.Include(server => server.GameServer).Single(
                            monitor => monitor.FileMonitorId == fileMonitorId);

                        var ftpUsername = fileMonitor.GameServer.FtpUsername;
                        var ftpPassword = fileMonitor.GameServer.FtpPassword;
                        var ftpHostname = fileMonitor.GameServer.FtpHostname;
                        var filePath = fileMonitor.FilePath;

                        try
                        {
                            var fileSize = GetFileSize(ftpUsername, ftpPassword, ftpHostname, filePath);

                            if (fileMonitor.BytesRead > fileSize ||
                                fileMonitor.LastRead < DateTime.UtcNow.AddMinutes(-5))
                            {
                                logger.Debug($"[{fileMonitorId}] Resetting bytes read to {fileMonitor.BytesRead}");
                                fileMonitor.BytesRead = fileSize;
                            }

                            var request = (FtpWebRequest) WebRequest.Create($"ftp://{ftpHostname}/{filePath}");
                            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                            request.ContentOffset = fileMonitor.BytesRead;
                            request.Method = WebRequestMethods.Ftp.DownloadFile;

                            ReadUntilError(context, fileMonitor, request);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message == "NoDataToRead")
                            {
                                fileMonitor.LastRead = DateTime.UtcNow;
                                context.SaveChanges();

                                logger.Debug($"[{fileMonitorId}] [{fileMonitor.BytesRead}] No data to read - waiting for 30 seconds...");

                                lastLoop = DateTime.UtcNow;
                                continue;
                            }

                            logger.Error(ex, $"[{fileMonitorId}] Failed monitoring remote file");
                            databaseLogger.CreateSystemLogAsync("Error",
                                $"[{fileMonitorId}] File Monitor failed monitoring remote file for {fileMonitor.FileMonitorId}",
                                ex.Message);
                            fileMonitor.LastError = ex.Message;
                            context.SaveChanges();
                        }

                        lastLoop = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    var fileMonitor = context.FileMonitors.Include(server => server.GameServer).Single(
                        monitor => monitor.FileMonitorId == fileMonitorId);

                    logger.Error(ex,
                        $"[{fileMonitorId}] Top level error monitoring file for {fileMonitor.FileMonitorId}");
                    databaseLogger.CreateSystemLogAsync("Error",
                        $"[{fileMonitorId}] File Monitor top level error monitoring for {fileMonitor.FileMonitorId}",
                        ex.Message);
                    fileMonitor.LastError = ex.Message;
                    context.SaveChanges();
                }
            }
        }

        private void ReadUntilError(DbContext context,
            Data.Core.Models.FileMonitor fileMonitor, WebRequest request)
        {
            using (var stream = request.GetResponse().GetResponseStream())
            {
                using (var sr = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    var prev = -1;

                    var byteList = new List<byte>();

                    while (true)
                    {
                        var cur = sr.Read();

                        if (cur == -1) throw new Exception("NoDataToRead");

                        byteList.Add((byte) cur);

                        if (prev == '\r' && cur == '\n')
                        {
                            fileMonitor.BytesRead += byteList.Count;
                            fileMonitor.LastRead = DateTime.UtcNow;
                            context.SaveChanges();
                            HandleLine(Encoding.Default.GetString(byteList.ToArray()),
                                fileMonitor.GameServer.ServerId);
                            byteList = new List<byte>();
                        }

                        prev = cur;
                    }
                }
            }
        }

        private static long GetFileSize(string username, string password, string hostname, string filePath)
        {
            var request = (FtpWebRequest) WebRequest.Create($"ftp://{hostname}/{filePath}");
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            var fileSize = ((FtpWebResponse) request.GetResponse()).ContentLength;
            return fileSize;
        }

        private void HandleLine(string line, Guid serverId)
        {
            line = line.Replace("\r\n", "");
            line = line.Trim();
            line = line.Substring(line.IndexOf(' ') + 1);

            try
            {
                if (line.StartsWith("J;"))
                    HandleJoinLine(line, serverId);
                else if (line.StartsWith("L;"))
                    HandleLeaveLine(line, serverId);
                else if (line.StartsWith("say;"))
                    HandleSayLine(line, ChatType.All, serverId);
                else if (line.StartsWith("sayteam;"))
                    HandleSayLine(line, ChatType.Team, serverId);
            }
            catch (Exception ex)
            {
                databaseLogger.CreateSystemLogAsync("Exception", $"Failed to handle line {line}", ex.Message);
            }


            logger.Write(LogEventLevel.Debug, line);
        }

        private void HandleSayLine(string line, ChatType chatType, Guid serverId)
        {
            var parts = line.Split(';');
            var guid = parts[1];
            var name = parts[3];
            var message = parts[4];
            message = new string(message.Where(c => !char.IsControl(c)).ToArray());

            EnsurePlayerExists(serverId, guid, name);

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == serverId);

                var chatLog = new ChatLog
                {
                    GameServer = gameServer,
                    Player = context.Players.Single(player =>
                        player.Guid == guid && player.GameType == gameServer.GameType),
                    Username = name,
                    ChatType = chatType,
                    Message = message,
                    Timestamp = DateTime.UtcNow
                };

                context.ChatLogs.Add(chatLog);
                context.SaveChanges();
            }
        }

        private void HandleLeaveLine(string line, Guid serverId)
        {
            var parts = line.Split(';');
            var guid = parts[1];
            var name = parts[3];

            EnsurePlayerExists(serverId, guid, name);
        }

        private void HandleJoinLine(string line, Guid serverId)
        {
            var parts = line.Split(';');
            var guid = parts[1];
            var name = parts[3];

            EnsurePlayerExists(serverId, guid, name);
        }

        private void EnsurePlayerExists(Guid serverId, string guid, string name)
        {
            if (guid == "allies" || guid == "axis")
                throw new Exception("Guid cannot be allies or axis - invalid");

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == serverId);

                var player = context.Players.SingleOrDefault(p =>
                    p.Guid == guid && p.GameType == gameServer.GameType);

                if (player == null)
                {
                    var playerToAdd = new Player2
                    {
                        GameType = gameServer.GameType,
                        Username = name,
                        Guid = guid,
                        FirstSeen = DateTime.UtcNow,
                        LastSeen = DateTime.UtcNow
                    };

                    context.Players.Add(playerToAdd);
                    databaseLogger.CreateSystemLogAsync("Info",
                        $"New player created for {gameServer.GameType} with username {name} and guid {guid}");
                }
                else
                {
                    var playerAlias = context.PlayerAliases.SingleOrDefault(pa => pa.Player.PlayerId == player.PlayerId && pa.Name == name);

                    if (playerAlias == null)
                    {
                        playerAlias = new PlayerAlias
                        {
                            Name = name,
                            Added = DateTime.UtcNow,
                            LastUsed = DateTime.UtcNow,
                            Player = player
                        };

                        context.PlayerAliases.Add(playerAlias);
                    }
                    else
                    {
                        playerAlias.LastUsed = DateTime.UtcNow;
                    }

                    if (player.Username != name) databaseLogger.CreateSystemLogAsync("Info", $"New name {name} recorded for {player.PlayerId}");

                    player.LastSeen = DateTime.UtcNow;
                    player.Username = name;
                }

                context.SaveChanges();
            }
        }
    }
}