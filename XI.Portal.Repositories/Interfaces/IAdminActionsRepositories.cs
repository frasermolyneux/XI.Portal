using System.Threading.Tasks;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Interfaces
{
    public interface IAdminActionsRepositories
    {
        Task<int> GetActiveBansCount();
        Task<int> GetUnclaimedBansCount();
        Task<int> GetActiveBansCount(GameType playerGame);
    }
}
