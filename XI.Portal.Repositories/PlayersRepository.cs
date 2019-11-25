using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
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

        public async Task<int> GetTrackedPlayerCount()
        {
            using (var context = contextProvider.GetContext())
            {
                return await context.Players.CountAsync();
            }
        }

        public async Task<int> GetTrackedPlayerCount(GameType playerGame)
        {
            using (var context = contextProvider.GetContext())
            {
                return await context.Players.Where(p => p.GameType == playerGame).Distinct().CountAsync();
            }
        }
    }
}
