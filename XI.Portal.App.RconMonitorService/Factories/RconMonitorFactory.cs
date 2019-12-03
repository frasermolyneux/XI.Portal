using System;
using System.Threading;
using Serilog;
using XI.Portal.App.RconMonitorService.Interfaces;
using XI.Portal.App.RconMonitorService.RconMonitors;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Plugins.MapRotationPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;

namespace XI.Portal.App.RconMonitorService.Factories
{
    internal class RconMonitorFactory : IRconMonitorFactory
    {
        private readonly ILogger logger;
        private readonly MapRotationPlugin mapRotationPlugin;
        private readonly PlayerInfoPlugin playerInfoPlugin;
        private readonly IRconClientFactory rconClientFactory;

        public RconMonitorFactory(ILogger logger, IRconClientFactory rconClientFactory, MapRotationPlugin mapRotationPlugin, PlayerInfoPlugin playerInfoPlugin)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
            this.mapRotationPlugin = mapRotationPlugin ?? throw new ArgumentNullException(nameof(mapRotationPlugin));
            this.playerInfoPlugin = playerInfoPlugin ?? throw new ArgumentNullException(nameof(playerInfoPlugin));
        }

        public IRconMonitor CreateInstance(GameType gameType, Guid serverId, string serverName, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource)
        {
            IRconMonitor rconMonitor;
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    rconMonitor = new Cod2RconMonitor(logger, rconClientFactory);
                    break;
                case GameType.CallOfDuty4:
                    rconMonitor = new Cod4RconMonitor(logger, rconClientFactory);
                    break;
                case GameType.CallOfDuty5:
                    rconMonitor = new Cod5RconMonitor(logger, rconClientFactory);
                    break;
                case GameType.Insurgency:
                    rconMonitor = new InsurgencyRconMonitor(logger, rconClientFactory);
                    break;
                default:
                    throw new Exception("Game type is not supported by the Rcon Monitor");
            }

            rconMonitor.Configure(serverId, serverName, gameType, hostname, port, rconPassword, monitorMapRotation, monitorPlayers, monitorPlayerIPs, cancellationTokenSource);
            mapRotationPlugin.RegisterEventHandlers(rconMonitor);
            playerInfoPlugin.RegisterEventHandlers(rconMonitor);

            return rconMonitor;
        }
    }
}