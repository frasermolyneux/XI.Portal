using System.Collections.Generic;

namespace XI.Portal.BLL.ViewModels
{
    public class PortalIndexViewModel
    {
        public int TrackedPlayerCount { get; internal set; }
        public int OnlinePlayerCount { get; internal set; }
        public int ActiveBanCount { get; internal set; }
        public int UnclaimedBanCount { get; internal set; }
        public List<GameIndexViewModel> GameIndexViewModels { get; internal set; }
    }
}
