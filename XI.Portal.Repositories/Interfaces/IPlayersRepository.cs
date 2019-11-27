using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Interfaces
{
    public interface IPlayersRepository
    {
        Task<GameType[]> GetPlayerGames();
        Task<int> GetPlayerCount(GetPlayersFilterModel filterModel);
        Task<List<Player2>> GetPlayers(GetPlayersFilterModel filterModel);
    }
}
