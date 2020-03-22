using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Contracts.Repositories
{
    public interface IAdminActionsRepository
    {
        Task<int> GetAdminActionsCount(AdminActionsFilterModel filterModel);
        Task<List<AdminAction>> GetAdminActions(AdminActionsFilterModel filterModel);
    }
}
