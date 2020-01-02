using System;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Monitor
{
    public class FileMonitorStatusViewModel
    {
        public FileMonitor FileMonitor { get; set; }
        public GameServer GameServer { get; set; }
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public bool SomethingMayBeWrong { get; set; } = false;
    }
}