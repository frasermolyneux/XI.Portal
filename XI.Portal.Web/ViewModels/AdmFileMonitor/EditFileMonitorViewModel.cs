using System;
using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.AdmFileMonitor
{
    public class EditFileMonitorViewModel
    {
        public List<GameServer> GameServers { get; set; }
        public Guid GameServerId { get; set; }
        public Guid FileMonitorId { get; set; }
        public string FilePath { get; set; }
    }
}