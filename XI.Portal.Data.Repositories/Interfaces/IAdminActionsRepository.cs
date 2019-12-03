using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Repositories.Interfaces
{
    public interface IAdminActionsRepository
    {
        Task<int> GetAdminActionsCount(AdminActionsFilterModel filterModel);
        Task<List<AdminAction>> GetAdminActions(AdminActionsFilterModel filterModel);
    }
}
