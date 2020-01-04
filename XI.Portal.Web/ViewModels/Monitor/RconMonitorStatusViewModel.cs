using System;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Monitor
{
    public class RconMonitorStatusViewModel
    {
        public RconMonitor RconMonitor { get; set; }
        public GameServer GameServer { get; set; }

        public string RconStatusResult { get; set; }

        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string SuccessMessage { get; set; } = "Everything looks OK";
    }
}