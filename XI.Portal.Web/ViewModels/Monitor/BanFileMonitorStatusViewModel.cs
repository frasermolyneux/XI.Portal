using System;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Monitor
{
    public class BanFileMonitorStatusViewModel
    {
        public BanFileMonitor BanFileMonitor { get; set; }
        public GameServer GameServer { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime LastGameBan { get; set; }
        public bool OutOfSync { get; set; } = false;
    }
}