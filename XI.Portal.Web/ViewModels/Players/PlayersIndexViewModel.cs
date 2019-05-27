using System.Collections.Generic;

namespace XI.Portal.Web.ViewModels.Players
{
    public class PlayersIndexViewModel
    {
        public int TotalTrackedPlayers { get; set; }
        public int TotalOnlinePlayers { get; set; }
        public int TotalBannedPlayers { get; set; }
        public int TotalUnclaimedBans { get; set; }

        public List<GameIndexViewModel> GameIndexViewModels { get; set; } = new List<GameIndexViewModel>();
    }
}