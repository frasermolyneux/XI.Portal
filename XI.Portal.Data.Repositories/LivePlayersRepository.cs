using System.Data.Entity;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;

namespace XI.Portal.Data.Repositories
{
    public class LivePlayersRepository : ILivePlayersRepository
    {
        private readonly IContextProvider contextProvider;

        public LivePlayersRepository(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new System.ArgumentNullException(nameof(contextProvider));
        }

        public async Task<int> GetOnlinePlayerCount()
        {
            using (var context = contextProvider.GetContext())
            {
                return await context.LivePlayers.CountAsync();
            }
        }
    }
}
