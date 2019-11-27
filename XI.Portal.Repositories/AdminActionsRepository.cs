using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Repositories.Extensions;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.Repositories
{
    public class AdminActionsRepository : IAdminActionsRepository
    {
        private readonly IContextProvider contextProvider;

        public AdminActionsRepository(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task<int> GetAdminActionsCount(AdminActionsFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).CountAsync();
            }
        }

        public async Task<List<AdminAction>> GetAdminActions(AdminActionsFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).ToListAsync();
            }
        }
    }
}
