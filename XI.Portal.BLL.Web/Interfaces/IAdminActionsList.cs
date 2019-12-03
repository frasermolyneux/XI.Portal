using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Data.Contracts.FilterModels;

namespace XI.Portal.BLL.Web.Interfaces
{
    public interface IAdminActionsList
    {
        Task<int> GetAdminActionsListCount(AdminActionsFilterModel filterModel);
        Task<List<AdminActionListEntryViewModel>> GetAdminActionsList(AdminActionsFilterModel filterModel);
    }
}
