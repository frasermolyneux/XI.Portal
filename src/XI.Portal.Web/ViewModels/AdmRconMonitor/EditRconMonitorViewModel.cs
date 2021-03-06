﻿using System;
using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.AdmRconMonitor
{
    public class EditRconMonitorViewModel
    {
        public List<GameServer> GameServers { get; set; }
        public Guid GameServerId { get; set; }
        public Guid RconMonitorId { get; set; }
        public bool MonitorMapRotation { get; set; }
        public bool MonitorPlayers { get; set; }
        public bool MonitorPlayerIPs { get; set; }
    }
}