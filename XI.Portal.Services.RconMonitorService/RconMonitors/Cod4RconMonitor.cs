﻿using System;
using Serilog;
using XI.Portal.Library.Rcon.Client;
using XI.Portal.Plugins.Events;

namespace XI.Portal.Services.RconMonitorService.RconMonitors
{
    internal class Cod4RconMonitor : BaseRconMonitor
    {
        private readonly ILogger logger;

        public Cod4RconMonitor(ILogger logger) : base(logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void GetMapRotation()
        {
            var rconClient = new RconClient(Hostname, Port, RconPassword);

            try
            {
                var commandResponse = rconClient.MapRotation();

                OnMapRotationRconResponse(new OnMapRotationRconResponse()
                {
                    ServerId = ServerId,
                    ResponseData = commandResponse
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"[{ServerId}] Failed to retrieve map rotation");
            }

            base.GetMapRotation();
        }
    }
}