using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Analytics.Interfaces;
using XI.Portal.Library.Analytics.Models;

namespace XI.Portal.Library.Analytics.Providers
{
    public class PlayersAnalytics : IPlayersAnalytics
    {
        private readonly IContextProvider contextProvider;

        public PlayersAnalytics(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task<List<PlayerAnalyticEntry>> GetCumulativeDailyPlayers(DateTime cutoff)
        {
            using (var context = contextProvider.GetContext())
            {
                var players = await context.Players
                    .Where(p => p.FirstSeen > cutoff)
                    .Select(p => p.FirstSeen)
                    .OrderBy(p => p)
                    .ToListAsync();

                var cumulative = 0;
                var groupedPlayers = players.GroupBy(p => new DateTime(p.Year, p.Month, p.Day))
                    .Select(g => new PlayerAnalyticEntry
                    {
                        Created = g.Key,
                        Count = cumulative += g.Count()
                    })
                    .ToList();

                return groupedPlayers;
            }
        }
    }
}
