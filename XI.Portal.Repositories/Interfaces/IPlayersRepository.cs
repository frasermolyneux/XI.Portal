using System.Threading.Tasks;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Interfaces
{
    public interface IPlayersRepository
    {
        Task<int> GetTrackedPlayerCount();
        Task<int> GetTrackedPlayerCount(GameType playerGame);
        Task<GameType[]> GetPlayerGames();
    }
}
