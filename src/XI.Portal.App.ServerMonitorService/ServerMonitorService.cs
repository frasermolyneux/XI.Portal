using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Serilog;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Logging;
using XI.Portal.Library.ServerInfo;

namespace XI.Portal.App.ServerMonitorService
{
    internal class ServerMonitorService
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly ILogger logger;

        private readonly Dictionary<Guid, Thread> workerThreads;
        private Thread controllingThread;

        private CancellationTokenSource cts;

        public ServerMonitorService(ILogger logger, IContextProvider contextProvider,
            IDatabaseLogger databaseLogger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));

            workerThreads = new Dictionary<Guid, Thread>();
        }

        public void Start()
        {
            logger.Information("Starting new Server Monitor instance");

            cts = new CancellationTokenSource();
            controllingThread = new Thread(() => StartControllingThread(cts.Token));
            controllingThread.Start();
        }

        public void Stop()
        {
            logger.Information("Stopping current Server Monitor instance");

            cts?.Cancel();

            foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
        }

        public void Shutdown()
        {
            logger.Information("Shutting down current Server Monitor instance");

            cts?.Cancel();

            foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
        }

        private void StartControllingThread(CancellationToken token)
        {
            logger.Information("Server Monitor controlling thread started");

            var lastLoop = DateTime.MinValue;
            while (!token.IsCancellationRequested)
            {
                if (DateTime.UtcNow < lastLoop.AddSeconds(30))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    using (var context = contextProvider.GetContext())
                    {
                        var serverMonitors = context.GameServers.Where(server =>
                            server.ShowOnPortalServerList && server.GameType != GameType.Unknown).ToList();

                        foreach (var serverMonitor in serverMonitors)
                            if (workerThreads.ContainsKey(serverMonitor.ServerId))
                            {
                                var workerThread = workerThreads[serverMonitor.ServerId];

                                if (workerThread != null && workerThread.IsAlive) continue;
                                var thread = new Thread(() =>
                                    MonitorServer(cts.Token, serverMonitor.ServerId));
                                thread.Start();

                                logger.Information("[{serverName}] Recreated worker thread for server",
                                    serverMonitor.Title);

                                workerThreads[serverMonitor.ServerId] = thread;
                            }
                            else
                            {
                                var thread = new Thread(() =>
                                    MonitorServer(cts.Token, serverMonitor.ServerId));
                                thread.Start();

                                logger.Information("[{serverName}] Created worker thread for server",
                                    serverMonitor.Title);

                                workerThreads.Add(serverMonitor.ServerId, thread);
                            }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception around main loop - possible network issue");
                }

                lastLoop = DateTime.UtcNow;
            }
        }

        private void MonitorServer(CancellationToken token, Guid serverMonitorId)
        {
            try
            {
                var lastLoop = DateTime.MinValue;
                while (!token.IsCancellationRequested)
                {
                    if (DateTime.UtcNow < lastLoop.AddSeconds(60))
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    try
                    {
                        using (var context = contextProvider.GetContext())
                        {
                            var serverMonitor =
                                context.GameServers.Single(monitor => monitor.ServerId == serverMonitorId);

                            try
                            {
                                var gameServerInfo = new GameServerInfo(serverMonitor.Hostname, serverMonitor.QueryPort,
                                    serverMonitor.GameType);
                                gameServerInfo.QueryServer();

                                serverMonitor.LiveTitle = gameServerInfo.Name;
                                serverMonitor.LiveMap = gameServerInfo.Map;
                                serverMonitor.LiveMod = gameServerInfo.Mod;
                                serverMonitor.LiveMaxPlayers = gameServerInfo.MaxPlayers;
                                serverMonitor.LiveCurrentPlayers = gameServerInfo.NumPlayers;
                                serverMonitor.LiveLastUpdated = DateTime.UtcNow;

                                var liveServerPlayers =
                                    context.LivePlayers.Where(player =>
                                        player.GameServer.ServerId == serverMonitor.ServerId);
                                context.LivePlayers.RemoveRange(liveServerPlayers);

                                foreach (LivePlayer player in gameServerInfo.Players)
                                {
                                    player.GameServer = serverMonitor;
                                    context.LivePlayers.Add(player);
                                }

                                logger.Information(
                                    "[{serverName}] Updated server live info with {map}, {mod} and {currentPlayers} players",
                                    serverMonitor.Title, gameServerInfo.Map, gameServerInfo.Mod,
                                    gameServerInfo.NumPlayers);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, "[{serverName}] Failed to get game server info", serverMonitor.Title);

                                serverMonitor.LiveMap = "Unknown";
                                serverMonitor.LiveMod = "Unknown";
                                serverMonitor.LiveCurrentPlayers = 0;
                                serverMonitor.LiveLastUpdated = DateTime.UtcNow;
                            }
                            finally
                            {
                                context.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"[{serverMonitorId}] Failed to store servers latest state");
                    }

                    lastLoop = DateTime.UtcNow;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{serverMonitorId}] Top level error monitoring");
            }
        }
    }
}