using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Monitor
{
    public class DirectQueryMonitorStatusViewModel
    {
        public GameServer GameServer { get; set; }

        public string Mod { get; set; }
        public string Map { get; set; }
        public int PlayerCount { get; set; }

        public string ErrorMessage { get; set; }
        public string WarningMessage { get; set; }
        public string SuccessMessage { get; set; } = "Everything looks OK";
    }
}