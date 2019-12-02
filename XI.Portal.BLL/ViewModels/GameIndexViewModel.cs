using XI.Portal.Library.CommonTypes;

namespace XI.Portal.BLL.ViewModels
{
    public class GameIndexViewModel
    {
        public GameType GameType { get; internal set; }
        public int TrackedPlayerCount { get; internal set; }
        public int ActiveBanCount { get; internal set; }
    }
}
