using System.Collections.Generic;
using XI.Portal.Library.Analytics.Models;

namespace XI.Portal.Web.ViewModels.Analytics
{
    public class PlayersAnalyticsViewModel
    {
        public List<PlayerAnalyticEntry> Players { get; set; }
        public List<PlayerAnalyticPerGameEntry> PlayersPerGame { get; set; }
    }
}