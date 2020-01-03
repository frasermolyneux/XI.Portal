using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.Library.Analytics.Models;

namespace XI.Portal.Library.Analytics.Interfaces
{
    public interface IPlayersAnalytics
    {
        Task<List<PlayerAnalyticEntry>> GetCumulativeDailyPlayers(DateTime cutoff);
    }
}
