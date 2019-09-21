using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Polly;
using Serilog;
using Topshelf;
using Topshelf.Unity;
using Unity;
using Unity.Lifetime;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.Configuration.Providers;
using XI.Portal.Library.GeoLocation.Extensions;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Client;
using XI.Portal.Library.Rcon.Source;

namespace XI.Portal.Services.RconMonitorService
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

            container.RegisterFactory<ILogger>((ctr, type, name) => logger, new ContainerControlledLifetimeManager());

            container.RegisterType<IContextProvider, ContextProvider>();
            container.RegisterType<IDatabaseLogger, DatabaseLogger>();

            container.RegisterType<IGeoLocationApiRepository, GeoLocationApiRepository>();

            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UseSerilog();

                x.Service<RconMonitorService>(s =>
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

        internal class RconMonitorService
        {
            private readonly IContextProvider contextProvider;
            private readonly IGeoLocationApiRepository geoLocationApiRepository;
            private readonly ILogger logger;

            private readonly Dictionary<Guid, Thread> workerThreads;
            private Thread controllingThread;

            private CancellationTokenSource cts;

            public RconMonitorService(ILogger logger, IContextProvider contextProvider, IGeoLocationApiRepository geoLocationApiRepository)
            {
                this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
                this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
                this.geoLocationApiRepository = geoLocationApiRepository ?? throw new ArgumentNullException(nameof(geoLocationApiRepository));

                workerThreads = new Dictionary<Guid, Thread>();
            }

            public void Start()
            {
                logger.Information("[RconMonitor] Starting new instance");

                cts = new CancellationTokenSource();
                controllingThread = new Thread(() => StartControllingThread(cts.Token));
                controllingThread.Start();
            }

            public void Stop()
            {
                logger.Information("[RconMonitor] Stopping current instance");

                cts?.Cancel();

                foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
            }

            public void Shutdown()
            {
                logger.Information("[RconMonitor] Shutting down current instance");

                cts?.Cancel();

                foreach (var workerThread in workerThreads) workerThread.Value?.Abort();
            }

            private void StartControllingThread(CancellationToken token)
            {
                logger.Information("[RconMonitor] Starting controlling thread");

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
                        var rconMonitors = context.RconMonitors.Include(monitor => monitor.GameServer).ToList();

                        foreach (var rconMonitor in rconMonitors)
                            if (workerThreads.ContainsKey(rconMonitor.RconMonitorId))
                            {
                                var workerThread = workerThreads[rconMonitor.RconMonitorId];

                                if (workerThread != null && workerThread.IsAlive)
                                    continue;

                                var thread = new Thread(() => MonitorRcon(cts.Token, rconMonitor.RconMonitorId));
                                thread.Start();

                                logger.Information("[RconMonitor] Recreated thread {thread} for rcon monitor {monitor}", rconMonitor.RconMonitorId, rconMonitor.GameServer.Title);

                                workerThreads[rconMonitor.RconMonitorId] = thread;
                            }
                            else
                            {
                                var thread = new Thread(() =>
                                    MonitorRcon(cts.Token, rconMonitor.RconMonitorId));
                                thread.Start();

                                logger.Information("[RconMonitor] Created thread {thread} for rcon monitor {monitor}", rconMonitor.RconMonitorId, rconMonitor.GameServer.Title);

                                workerThreads.Add(rconMonitor.RconMonitorId, thread);
                            }
                    }

                    lastLoop = DateTime.UtcNow;
                }
            }

            private static IEnumerable<TimeSpan> GetRetryTimeSpans()
            {
                var random = new Random();

                return new[]
                {
                    TimeSpan.FromSeconds(random.Next(30, 60)),
                    TimeSpan.FromSeconds(random.Next(120, 240)),
                    TimeSpan.FromSeconds(random.Next(480, 600))
                };
            }

            private void MonitorRcon(CancellationToken token, Guid rconMonitorId)
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

                            var rconMonitor = context.RconMonitors.Include(server => server.GameServer)
                                .Single(monitor => monitor.RconMonitorId == rconMonitorId);

                            try
                            {
                                var rconHostname = rconMonitor.GameServer.Hostname;
                                var rconPort = rconMonitor.GameServer.QueryPort;
                                var rconPassword = rconMonitor.GameServer.RconPassword;

                                if (rconMonitor.MonitorPlayerIPs || rconMonitor.MonitorPlayers)
                                {
                                    if (rconMonitor.GameServer.GameType == GameType.Insurgency)
                                    {
                                        try
                                        {
                                            Policy.Handle<Exception>()
                                                .WaitAndRetry(GetRetryTimeSpans(), (result, timeSpan, retryCount, acontext) => { logger.Warning("[RconMonitor] Failed to execute status command for {server}. Retry attempt {retryCount}, waiting {timeSpan}", rconMonitor.GameServer.Title, retryCount, timeSpan); })
                                                .Execute(() => ExecuteSourcePlayerMonitor(rconMonitor, rconHostname, rconPort, rconPassword));
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex, "[RconMonitor] Failed to execute status command for {server}", rconMonitor.GameServer.Title);
                                        }
                                    }
                                    else
                                    {
                                        var rconClient = new RconClient(rconHostname, rconPort, rconPassword);

                                        var rconCommandResult = Policy
                                            .Handle<Exception>()
                                            .WaitAndRetry(GetRetryTimeSpans(), (result, timeSpan, retryCount, acontext) => { logger.Warning("[RconMonitor] Failed to execute status command for {server}. Retry attempt {retryCount}, waiting {timeSpan}", rconMonitor.GameServer.Title, retryCount, timeSpan); })
                                            .Execute(() => rconClient.StatusCommand());

                                        HandleCallOfDutyStatusResponse(rconCommandResult, rconMonitor.GameServer.ServerId, rconMonitor.GameServer.GameType);
                                    }
                                }

                                if (rconMonitor.MonitorMapRotation)
                                {
                                    if (!rconMonitor.MonitorMapRotation ||
                                        rconMonitor.MapRotationLastUpdated >= DateTime.UtcNow.AddHours(-1))
                                    {
                                        lastLoop = DateTime.UtcNow;
                                        continue;
                                    }

                                    var rconClient = new RconClient(rconHostname, rconPort, rconPassword);

                                    var mapRotationResult = Policy
                                        .Handle<Exception>()
                                        .WaitAndRetry(GetRetryTimeSpans(), (result, timeSpan, retryCount, acontext) => { logger.Warning("[RconMonitor] Failed to execute map rotation command for {server}. Retry attempt {retryCount}, waiting {timeSpan}", rconMonitor.GameServer.Title, retryCount, timeSpan); })
                                        .Execute(() => rconClient.MapRotation());

                                    HandleMapRotationResponse(mapRotationResult, rconMonitor.GameServer.ServerId, rconMonitor.GameServer.GameType);

                                    rconMonitor.MapRotationLastUpdated = DateTime.UtcNow;
                                    rconMonitor.LastError = string.Empty;
                                    context.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex, "[RconMonitor] Failed to process loop for {server}", rconMonitor.GameServer.Title);
                            }

                            lastLoop = DateTime.UtcNow;
                        }
                    }
                    catch (Exception ex)
                    {
                        var rconMonitor = context.RconMonitors.Include(server => server.GameServer)
                            .Single(monitor => monitor.RconMonitorId == rconMonitorId);

                        logger.Error(ex, "[RconMonitor] Failed with top-level exception for {server}", rconMonitor.GameServer.Title);

                        rconMonitor.LastError = ex.Message;
                        context.SaveChanges();
                    }
                }
            }

            private void ExecuteSourcePlayerMonitor(Data.Core.Models.RconMonitor rconMonitor, string rconHostname, int rconPort, string rconPassword)
            {
                var sourceRcon = new SourceRconClient(rconHostname, rconPort, rconPassword);

                sourceRcon.Errors += delegate(string output)
                {
                    var exception = new Exception(output);
                    logger.Error(exception, "[RconMonitor] Error executing source rcon command for {server}", rconMonitor.GameServer.Title);
                };

                sourceRcon.ServerOutput += delegate(string output) { HandleSourceStatusResponse(output, rconMonitor.GameServer.ServerId, rconMonitor.GameServer.GameType); };
            }

            private void HandleCallOfDutyStatusResponse(string statusCommandResult, Guid gameServerServerId, GameType gameServerGameType)
            {
                Regex regex;
                switch (gameServerGameType)
                {
                    case GameType.CallOfDuty2:
                        regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                        break;
                    case GameType.CallOfDuty4:
                        regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9a-f]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                        break;
                    case GameType.CallOfDuty5:
                        regex = new Regex("^\\s*([0-9]+)\\s+([0-9-]+)\\s+([0-9]+)\\s+([0-9]+)\\s+(.*?)\\s+([0-9]+?)\\s*((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})\\s*(-?[0-9]{1,5})\\s+([0-9]+)$");
                        break;
                    default:
                        throw new Exception("Game Type is not supported");
                }

                var lines = statusCommandResult.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

                using (var context = contextProvider.GetContext())
                {
                    for (var i = 3; i < lines.Count; i++)
                    {
                        var line = lines[i];
                        var match = regex.Match(line);

                        if (!match.Success)
                            continue;

                        var guid = match.Groups[4].ToString();
                        var ipAddress = match.Groups[7].ToString();

                        var player = context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == gameServerGameType);

                        if (player == null)
                            continue;

                        UpdatePlayerIpAddress(context, ipAddress, player);
                        UpdateLivePlayerIpAddress(context, ipAddress);
                    }

                    var livePlayerLocationCutOff = DateTime.UtcNow.AddDays(-1);
                    context.LivePlayerLocations.RemoveRange(context.LivePlayerLocations.Where(lpl => lpl.LastSeen < livePlayerLocationCutOff));
                    context.SaveChanges();
                }
            }

            private void UpdateLivePlayerIpAddress(PortalContext context, string ipAddress)
            {
                try
                {
                    var ipLocation = geoLocationApiRepository.GetLocation(ipAddress).Result;

                    if (!ipLocation.IsDefault())
                    {
                        var storedIpLocation = context.LivePlayerLocations.SingleOrDefault(lpl => lpl.IpAddress == ipAddress);

                        if (storedIpLocation == null)
                        {
                            storedIpLocation = new LivePlayerLocation
                            {
                                IpAddress = ipAddress,
                                LastSeen = DateTime.UtcNow,
                                Lat = ipLocation.Latitude,
                                Long = ipLocation.Longitude
                            };

                            context.LivePlayerLocations.Add(storedIpLocation);
                        }
                        else
                        {
                            storedIpLocation.LastSeen = DateTime.UtcNow;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "[RconMonitor] Failed to get location for {ipAddress}", ipAddress);
                }
            }

            private void UpdatePlayerIpAddress(PortalContext context, string ipAddress, Player2 player)
            {
                var playerIpAddress = context.PlayerIpAddresses.SingleOrDefault(pip => pip.Player.PlayerId == player.PlayerId && pip.Address == ipAddress);

                if (playerIpAddress == null)
                {
                    playerIpAddress = new PlayerIpAddress
                    {
                        Address = ipAddress,
                        Added = DateTime.UtcNow,
                        LastUsed = DateTime.UtcNow,
                        Player = player
                    };

                    context.PlayerIpAddresses.Add(playerIpAddress);
                }
                else
                {
                    playerIpAddress.LastUsed = DateTime.UtcNow;
                }

                if (player.IpAddress != ipAddress)
                    logger.Information("[RconMonitor] New IpAddress {ipAddress} recorded for {PlayerId} ({Username})", ipAddress, player.PlayerId, player.Username);

                player.LastSeen = DateTime.UtcNow;
                player.IpAddress = ipAddress;
            }

            private void HandleSourceStatusResponse(string statusCommandResult, Guid gameServerServerId, GameType gameServerGameType)
            {
                var regex = new Regex("^\\#\\s([0-9]+)\\s([0-9]+)\\s\\\"(.+)\\\"\\s([STEAM0-9:_]+)\\s+([0-9:]+)\\s([0-9]+)\\s([0-9]+)\\s([a-z]+)\\s([0-9]+)\\s((?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9]?[0-9])):?(-?[0-9]{1,5})");

                var lines = statusCommandResult.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

                using (var context = contextProvider.GetContext())
                {
                    for (var i = 7; i < lines.Count; i++)
                    {
                        var line = lines[i];
                        var match = regex.Match(line);

                        if (!match.Success)
                            continue;

                        var name = match.Groups[3].ToString();
                        var guid = match.Groups[4].ToString();
                        var ipAddress = match.Groups[10].ToString();

                        EnsurePlayerExists(gameServerServerId, guid, name);
                        var player = context.Players.SingleOrDefault(p => p.Guid == guid && p.GameType == gameServerGameType);
                        UpdatePlayerIpAddress(context, ipAddress, player);
                        UpdateLivePlayerIpAddress(context, ipAddress);

                        var livePlayerLocationCutOff = DateTime.UtcNow.AddDays(-1);
                        context.LivePlayerLocations.RemoveRange(context.LivePlayerLocations.Where(lpl => lpl.LastSeen < livePlayerLocationCutOff));
                        context.SaveChanges();
                    }
                }
            }

            private void HandleMapRotationResponse(string line, Guid serverId, GameType gameType)
            {
                // gametype ftag map mp_cgc_bog gametype ftag map mp_cgc_citystreets gametype ftag map mp_strike gametype ftag map mp_carentan gametype ftag
                // map mp_coldfront gametype ftag map mp_vac_2 gametype ftag map mp_cgc_crossfire gametype ftag map mp_overgrown gametype ftag map mp_crash gametype ftag
                // map mp_convoy gametype ftag map mp_4t4hangar gametype ftag map mp_farm gametype ftag map mp_pipeline gametype ftag map mp_shipment gametype ftag
                // map mp_countdown gametype ftag map mp_backlot gametype ftag map mp_bloc gametype ftag map mp_killhouse gametype ftag map mp_bog gametype ftag
                // map mp_cargoship gametype ftag map mp_tigertown_v2"

                //gametype dm map mp_airfield map mp_asylum map mp_castle map mp_shrine map mp_courtyard map mp_dome map mp_downfall map mp_hangar
                //map mp_makin map mp_outskirts map mp_roundhouse map mp_seelow map mp_suburban map mp_makin_day map mp_kneedeep map mp_nachtfeuer map mp_subway map mp_docks
                //map mp_kwai map mp_stalingrad map mp_drum map mp_bgate map mp_vodka"


                line = line.Replace("\"sv_mapRotation\" is: \"", "");
                line = line.Replace("^7\" default: \"^7\"", "");
                line = line.Replace("Domain is any text", "");
                line = line.Trim();

                var allParts = line.Split(' ').Where(part => !string.IsNullOrWhiteSpace(part));
                var gameTypeCount = allParts.Count(part => part.Trim() == "gametype");

                using (var context = contextProvider.GetContext())
                {
                    var gameServer = context.GameServers.Single(s => s.ServerId == serverId);
                    var currentMapRotation = context.MapRotations.Where(mr => mr.GameServer.ServerId == gameServer.ServerId);
                    context.MapRotations.RemoveRange(currentMapRotation);

                    var newMapRotation = new List<MapRotation>();
                    if (gameTypeCount > 1)
                    {
                        var parts = line.Split(new[] {"gametype"}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var part in parts)
                        {
                            var subParts = part.Split(new[] {"map"}, StringSplitOptions.RemoveEmptyEntries);
                            var gameMode = subParts.First().Trim();
                            var mapName = subParts.Skip(1).Single().Trim();

                            var map = context.Maps.SingleOrDefault(m =>
                                m.MapName == mapName && m.GameType == gameServer.GameType);

                            if (map == null)
                            {
                                logger.Warning("[RconMonitor] Could not find map {mapName} to add to rotation for {gameType}", mapName, gameServer.GameType);
                                continue;
                            }

                            newMapRotation.Add(new MapRotation
                            {
                                GameServer = gameServer,
                                Map = map,
                                GameMode = gameMode
                            });
                        }
                    }
                    else
                    {
                        line = line.Replace("gametype", "");
                        var parts = line.Split(new[] {"map"}, StringSplitOptions.RemoveEmptyEntries);
                        var gameMode = parts.First().Trim();
                        Console.WriteLine($"Game Mode: {gameMode}");
                        foreach (var part in parts.Skip(1))
                        {
                            var mapName = part.Trim();

                            var map = context.Maps.SingleOrDefault(m =>
                                m.MapName == mapName && m.GameType == gameServer.GameType);

                            if (map == null)
                            {
                                logger.Warning("[RconMonitor] Could not find map {mapName} to add to rotation for {gameType}", mapName, gameServer.GameType);
                                continue;
                            }

                            newMapRotation.Add(new MapRotation
                            {
                                GameServer = gameServer,
                                Map = map,
                                GameMode = gameMode
                            });
                        }
                    }

                    context.MapRotations.AddRange(newMapRotation);
                    context.SaveChanges();

                    logger.Information("[RconMonitor] Updated map rotation for {serverId} with {mapCount} maps", serverId, newMapRotation.Count);
                }
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
                        logger.Information("[RconMonitor] New player created for {GameType} with username {name} and guid {guid}", gameServer.GameType, name, guid);
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

                        if (player.Username != name)
                            logger.Information("[RconMonitor] New name {name} recorded for {PlayerId}", name, player.PlayerId);

                        player.LastSeen = DateTime.UtcNow;
                        player.Username = name;
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}