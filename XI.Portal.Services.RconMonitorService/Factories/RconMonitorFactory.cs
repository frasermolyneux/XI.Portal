using System;
using System.Threading;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.MapRotationPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;
using XI.Portal.Services.RconMonitorService.Interfaces;
using XI.Portal.Services.RconMonitorService.RconMonitors;

namespace XI.Portal.Services.RconMonitorService.Factories
{
    internal class RconMonitorFactory : IRconMonitorFactory
    {
        private readonly ILogger logger;
        private readonly MapRotationPlugin mapRotationPlugin;
        private readonly PlayerInfoPlugin playerInfoPlugin;

        public RconMonitorFactory(ILogger logger, MapRotationPlugin mapRotationPlugin, PlayerInfoPlugin playerInfoPlugin)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapRotationPlugin = mapRotationPlugin ?? throw new ArgumentNullException(nameof(mapRotationPlugin));
            this.playerInfoPlugin = playerInfoPlugin ?? throw new ArgumentNullException(nameof(playerInfoPlugin));
        }

        public IRconMonitor CreateInstance(GameType gameType, Guid serverId, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource)
        {
            IRconMonitor rconMonitor;
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    rconMonitor = new Cod2RconMonitor(logger);
                    break;
                case GameType.CallOfDuty4:
                    rconMonitor = new Cod4RconMonitor(logger);
                    break;
                case GameType.CallOfDuty5:
                    rconMonitor = new Cod5RconMonitor(logger);
                    break;
                default:
                    throw new Exception("Game type is not supported by the Rcon Monitor");
            }

            rconMonitor.Configure(serverId, gameType, hostname, port, rconPassword, monitorMapRotation, monitorPlayers, monitorPlayerIPs, cancellationTokenSource);
            mapRotationPlugin.RegisterEventHandlers(rconMonitor);
            playerInfoPlugin.RegisterEventHandlers(rconMonitor);

            return rconMonitor;
        }
    }
}