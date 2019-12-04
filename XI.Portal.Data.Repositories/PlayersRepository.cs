using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Data.Repositories.Extensions;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Data.Repositories
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

        public async Task<int> GetPlayerCount(PlayersFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).CountAsync();
            }
        }

        public async Task<List<Player2>> GetPlayers(PlayersFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).ToListAsync();
            }
        }
    }
}
