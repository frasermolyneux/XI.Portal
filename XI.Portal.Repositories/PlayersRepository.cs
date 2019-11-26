using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.Repositories
{
    public class PlayersRepository : IPlayersRepository
    {
        private readonly IContextProvider contextProvider;

        public PlayersRepository(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new System.ArgumentNullException(nameof(contextProvider));
        }

        public async Task<GameType[]> GetPlayerGames()
        {
            using (var context = contextProvider.GetContext())
            {
                return await context.Players.Select(p => p.GameType).Distinct().ToArrayAsync();
            }
        }

        public async Task<int> GetPlayerCount(GameType gameType = GameType.Unknown, string filterType = null, string filterString = null)
        {
            using (var context = contextProvider.GetContext())
            {
                var players = context.Players.AsQueryable();

                if (gameType != GameType.Unknown)
                {
                    players = players.Where(p => p.GameType == gameType).AsQueryable();
                }

                if (!string.IsNullOrWhiteSpace(filterType) && !string.IsNullOrWhiteSpace(filterString))
                {
                    switch (filterType)
                    {
                        case "UsernameAndGuid":
                            players = players.Where(p => p.Username.Contains(filterString) || p.Guid.Contains(filterString)).AsQueryable();
                            break;
                        case "IpAddress":
                            players = players.Where(p => p.IpAddress.Contains(filterString)).AsQueryable();
                            break;
                        default:
                            break;
                    }
                }

                return await players.CountAsync();
            }
        }

        public async Task<List<Player2>> GetPlayers(GameType gameType = GameType.Unknown, string filterType = null, string filterString = null, string orderBy = null, int playersToSkip = 0, int entriesToTake = 20)
        {
            using (var context = contextProvider.GetContext())
            {
                var players = context.Players.AsQueryable();

                if (gameType != GameType.Unknown)
                {
                    players = players.Where(p => p.GameType == gameType).AsQueryable();
                }

                if (!string.IsNullOrWhiteSpace(filterType) && !string.IsNullOrWhiteSpace(filterString))
                {
                    switch (filterType)
                    {
                        case "UsernameAndGuid":
                            players = players.Where(p => p.Username.Contains(filterString) || p.Guid.Contains(filterString)).AsQueryable();
                            break;
                        case "IpAddress":
                            players = players.Where(p => p.IpAddress.Contains(filterString)).AsQueryable();
                            break;
                        default:
                            break;
                    }
                }

                var orderByField = orderBy ?? "";

                players = players.Skip(playersToSkip).Take(entriesToTake).AsQueryable();

                return await players.ToListAsync();
            }
        }
    }
}
