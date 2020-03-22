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

        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string SuccessMessage { get; set; } = "Everything looks OK";
    }
}