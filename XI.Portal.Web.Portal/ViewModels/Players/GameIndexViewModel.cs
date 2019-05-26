﻿using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Portal.ViewModels.Players
{
    public class GameIndexViewModel
    {
        public GameType GameType { get; set; }
        public int TrackedPlayers { get; set; }
        public int BannedPlayers { get; set; }
    }
}