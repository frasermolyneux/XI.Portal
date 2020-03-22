using System;
using System.Data.Entity;
using System.Threading;
using Serilog;
using XI.Portal.App.RconMonitorService.Interfaces;
using XI.Portal.Data.Core.Context;

namespace XI.Portal.App.RconMonitorService
{
    internal partial class Program
    {
        internal class RconMonitorAppService
        {
            private readonly IContextProvider contextProvider;
            private readonly IRconMonitorFactory rconMonitorFactory;
            private readonly ILogger logger;

            private CancellationTokenSource cts;

            public RconMonitorAppService(ILogger logger, IContextProvider contextProvider, IRconMonitorFactory rconMonitorFactory)
            {
                this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
                this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
                this.rconMonitorFactory = rconMonitorFactory ?? throw new ArgumentNullException(nameof(rconMonitorFactory));
            }

            public void Start()
            {
                logger.Information("[RconMonitor] Starting new instance");

                cts = new CancellationTokenSource();

                using (var context = contextProvider.GetContext())
                {
                    foreach (var rconMonitor in context.RconMonitors.Include(rm => rm.GameServer))
                    {
                        logger.Information($"Creating RconMonitor for {rconMonitor.GameServer.Title}");

                        try
                        {
                            rconMonitorFactory.CreateInstance(
                                rconMonitor.RconMonitorId,
                                rconMonitor.GameServer.GameType,
                                rconMonitor.GameServer.ServerId,
                                rconMonitor.GameServer.Title,
                                rconMonitor.GameServer.Hostname,
                                rconMonitor.GameServer.QueryPort,
                                rconMonitor.GameServer.RconPassword,
                                rconMonitor.MonitorMapRotation,
                                rconMonitor.MonitorPlayers,
                                rconMonitor.MonitorPlayerIPs,
                                cts);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Failed to create instance for {serverName}", rconMonitor.GameServer.Title);
                        }
                    }
                }
            }

            public void Stop()
            {
                logger.Information("[RconMonitor] Stopping current instance");
                cts?.Cancel();
            }

            public void Shutdown()
            {
                logger.Information("[RconMonitor] Shutting down current instance");
                cts?.Cancel();
            }
        }
    }
}