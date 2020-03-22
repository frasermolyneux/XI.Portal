using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Models;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Data.Contracts.Repositories
{
    public interface IPlayersRepository
    {
        Task<GameType[]> GetPlayerGames();
        Task<int> GetPlayerCount(PlayersFilterModel filterModel);
        Task<List<Player2>> GetPlayers(PlayersFilterModel filterModel);
    }
}
