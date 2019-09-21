using System;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Plugins.Events;

namespace XI.Portal.Services.RconMonitorService.RconMonitors
{
    internal class InsurgencyRconMonitor : BaseRconMonitor
    {
        private readonly ILogger logger;
        private readonly IRconClientFactory rconClientFactory;

        public InsurgencyRconMonitor(ILogger logger, IRconClientFactory rconClientFactory) : base(logger, rconClientFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
        }


        public override void GetMapRotation()
        {
            var rconClient = rconClientFactory.CreateInstance(GameType.Insurgency, Hostname, Port, RconPassword);

            try
            {
                var commandResponse = rconClient.MapRotation();

                OnMapRotationRconResponse(new OnMapRotationRconResponse(ServerId, GameType, commandResponse));
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{ServerId}] Failed to retrieve server status");
            }

            base.GetMapRotation();
        }

        public override void GetStatus()
        {
            var rconClient = rconClientFactory.CreateInstance(GameType.Insurgency, Hostname, Port, RconPassword);

            try
            {
                var commandResponse = rconClient.PlayerStatus();

                OnStatusRconResponse(new OnStatusRconResponse(ServerId, GameType, commandResponse, MonitorPlayers, MonitorPlayerIPs));
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{ServerId}] Failed to retrieve server status");
            }

            base.GetStatus();
        }
    }
}