using System;
using Serilog;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Plugins.Events;

namespace XI.Portal.App.RconMonitorService.RconMonitors
{
    internal class Cod4RconMonitor : BaseRconMonitor
    {
        private readonly ILogger logger;
        private readonly IRconClientFactory rconClientFactory;

        public Cod4RconMonitor(ILogger logger, IRconClientFactory rconClientFactory) : base(logger, rconClientFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
        }


        public override void GetMapRotation()
        {
            var rconClient = rconClientFactory.CreateInstance(GameType.CallOfDuty4, ServerName, Hostname, Port, RconPassword);

            try
            {
                var commandResponse = rconClient.MapRotation();

                OnMapRotationRconResponse(new OnMapRotationRconResponse(MonitorId, ServerId, ServerName, GameType, commandResponse));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[{serverName}] Failed to retrieve map rotation", ServerName);
            }

            base.GetMapRotation();
        }

        public override void GetStatus()
        {
            var rconClient = rconClientFactory.CreateInstance(GameType.CallOfDuty4, ServerName, Hostname, Port, RconPassword);

            try
            {
                var commandResponse = rconClient.PlayerStatus();

                OnStatusRconResponse(new OnStatusRconResponse(MonitorId, ServerId, ServerName, GameType, commandResponse, MonitorPlayers, MonitorPlayerIPs));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[{serverName}] Failed to retrieve server status", ServerName);
            }

            base.GetStatus();
        }
    }
}