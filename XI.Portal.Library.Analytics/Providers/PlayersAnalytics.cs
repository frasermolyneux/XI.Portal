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
                var cumulative = await context.Players.CountAsync(p => p.FirstSeen < cutoff);

                var players = await context.Players
                    .Where(p => p.FirstSeen > cutoff)
                    .Select(p => p.FirstSeen)
                    .OrderBy(p => p)
                    .ToListAsync();

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

        public async Task<List<PlayerAnalyticPerGameEntry>> GetNewDailyPlayersPerGame(DateTime cutoff)
        {
            using (var context = contextProvider.GetContext())
            {
                var players = await context.Players
                    .Where(p => p.FirstSeen > cutoff)
                    .Select(p => new { p.FirstSeen, p.GameType })
                    .OrderBy(p => p)
                    .ToListAsync();

                var groupedPlayers = players.GroupBy(p => new DateTime(p.FirstSeen.Year, p.FirstSeen.Month, p.FirstSeen.Day))
                    .Select(g => new PlayerAnalyticPerGameEntry
                    {
                        Created = g.Key,
                        GameCounts = g.GroupBy(i => i.GameType.ToString())
                            .Select(i => new { Type = i.Key, Count = i.Count() })
                            .ToDictionary(a => a.Type, a => a.Count)
                    }).ToList();

                return groupedPlayers;
            }
        }
    }
}