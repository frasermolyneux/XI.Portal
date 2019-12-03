using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.BLL.Web
{
    public class AdminActionsList : IAdminActionsList
    {
        private readonly IAdminActionsRepository adminActionsRepository;

        public AdminActionsList(IAdminActionsRepository adminActionsRepository)
        {
            this.adminActionsRepository = adminActionsRepository ?? throw new System.ArgumentNullException(nameof(adminActionsRepository));
        }

        public async Task<int> GetAdminActionsListCount(AdminActionsFilterModel filterModel)
        {
            if (filterModel == null) filterModel = new AdminActionsFilterModel();

            return await adminActionsRepository.GetAdminActionsCount(filterModel);
        }

        public async Task<List<AdminActionListEntryViewModel>> GetAdminActionsList(AdminActionsFilterModel filterModel)
        {
            if (filterModel == null) filterModel = new AdminActionsFilterModel();

            var adminActions = await adminActionsRepository.GetAdminActions(filterModel);

            var adminActionsEntryViewModels = adminActions.Select(aa => new AdminActionListEntryViewModel
            {
                PlayerId = aa.Player.PlayerId,
                Username = aa.Player.Username,
                Guid = aa.Player.Guid,
                Type = aa.Type.ToString(),
                Expires = aa.Type == AdminActionType.Ban ? "Never" : aa.Expires.ToString()
            }).ToList();

            return adminActionsEntryViewModels;
        }
    }
}
