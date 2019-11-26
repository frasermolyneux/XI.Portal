using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Interfaces
{
    public interface IPlayersRepository
    {
        Task<int> GetPlayerCount(GameType gameType = GameType.Unknown, string filterType = null, string filterString = null);
        Task<GameType[]> GetPlayerGames();
        Task<List<Player2>> GetPlayers(GameType gameType = GameType.Unknown, string filterType = null, string filterString = null, string orderBy = null, int playersToSkip = 0, int entriesToTake = 20);
    }
}
