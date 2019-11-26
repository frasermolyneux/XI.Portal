using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.Repositories
{
    public class AdminActionsRepositories : IAdminActionsRepositories
    {
        private readonly IContextProvider contextProvider;

        public AdminActionsRepositories(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task<int> GetActiveBansCount(GameType playerGame = GameType.Unknown)
        {
            using (var context = contextProvider.GetContext())
            {
                var adminActions = context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null
                    || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow).AsQueryable();

                if (playerGame != GameType.Unknown)
                {
                    adminActions = adminActions.Where(aa => aa.Player.GameType == playerGame).AsQueryable();
                }

                return await adminActions.CountAsync();
            }
        }

        public async Task<int> GetUnclaimedBansCount()
        {
            using (var context = contextProvider.GetContext())
            {
                return await context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null && aa.Admin == null)
                    .CountAsync();
            }
        }
    }
}
