using System;
using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.Portal.ViewModels.AdmFileMonitor
{
    public class CreateFileMonitorViewModel
    {
        public List<GameServer> GameServers { get; set; }
        public Guid GameServerId { get; set; }
        public string FilePath { get; set; }
    }
}