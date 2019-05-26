using System;
using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.Portal.ViewModels.AdmBanFileMonitor
{
    public class EditBanFileMonitorViewModel
    {
        public List<GameServer> GameServers { get; set; }
        public Guid GameServerId { get; set; }
        public Guid BanFileMonitorId { get; set; }
        public string FilePath { get; set; }
    }
}