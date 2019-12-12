using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Data.Repositories.Extensions;

namespace XI.Portal.Data.Repositories
{
    public class MapsRepository : IMapsRepository
    {
        private readonly IContextProvider contextProvider;

        public MapsRepository(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public async Task<int> GetMapCount(MapsFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).CountAsync();
            }
        }

        public async Task<List<Map>> GetMaps(MapsFilterModel filterModel)
        {
            using (var context = contextProvider.GetContext())
            {
                return await filterModel.ApplyFilter(context).Include(m => m.MapFiles).Include(m => m.MapVotes).ToListAsync();
            }
        }
    }
}
