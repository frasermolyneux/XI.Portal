using System.Threading.Tasks;

namespace XI.Portal.Repositories.Interfaces
{
    public interface ILivePlayersRepository
    {
        Task<int> GetOnlinePlayerCount();
    }
}
