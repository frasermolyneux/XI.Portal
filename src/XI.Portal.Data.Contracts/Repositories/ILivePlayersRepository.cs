using System.Threading.Tasks;

namespace XI.Portal.Data.Contracts.Repositories
{
    public interface ILivePlayersRepository
    {
        Task<int> GetOnlinePlayerCount();
    }
}
