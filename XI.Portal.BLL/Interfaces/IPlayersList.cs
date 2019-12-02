using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.BLL.ViewModels;

namespace XI.Portal.BLL.Interfaces
{
    public interface IPlayersList
    {
        Task<int> GetPlayerListCount(PlayersFilterModel filterModel = null);
        Task<List<PlayerListEntryViewModel>> GetPlayerList(PlayersFilterModel filterModel = null);
    }
}
