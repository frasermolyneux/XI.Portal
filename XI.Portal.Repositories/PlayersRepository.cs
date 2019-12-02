using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Repositories.Extensions;
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
