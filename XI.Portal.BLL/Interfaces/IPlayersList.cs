using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Models;
using XI.Portal.BLL.ViewModels;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.BLL.Interfaces
{
    public interface IPlayersList
    {
        Task<int> GetPlayerListCount(GameType gameType, PlayersListFilter playerListFilter, string filterString);
        Task<List<PlayerListEntryViewModel>> GetPlayerList(GameType gameType, PlayersListFilter playerListFilter, string filterString, int playersToSkip, int entriesToTake);
    }
}
