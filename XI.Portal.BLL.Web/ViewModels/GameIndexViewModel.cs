using XI.Portal.Data.CommonTypes;

namespace XI.Portal.BLL.Web.ViewModels
{
    public class GameIndexViewModel
    {
        public GameType GameType { get; internal set; }
        public int TrackedPlayerCount { get; internal set; }
        public int ActiveBanCount { get; internal set; }
    }
}
