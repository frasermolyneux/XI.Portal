using System;
using System.Collections.Generic;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Library.Analytics.Models
{
    public class PlayerAnalyticPerGameEntry
    {
        public DateTime Created { get; set; }
        public Dictionary<GameType, int> GameCounts { get; set; }
    }
}