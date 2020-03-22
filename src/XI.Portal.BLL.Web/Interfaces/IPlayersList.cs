using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Data.Contracts.FilterModels;

namespace XI.Portal.BLL.Web.Interfaces
{
    public interface IPlayersList
    {
        Task<int> GetPlayerListCount(PlayersFilterModel filterModel = null);
        Task<List<PlayerListEntryViewModel>> GetPlayerList(PlayersFilterModel filterModel = null);
    }
}
