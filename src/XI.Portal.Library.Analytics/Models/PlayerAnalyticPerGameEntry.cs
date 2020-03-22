using System;
using System.Collections.Generic;

namespace XI.Portal.Library.Analytics.Models
{
    public class PlayerAnalyticPerGameEntry
    {
        public DateTime Created { get; set; }
        public Dictionary<string, int> GameCounts { get; set; }
    }
}