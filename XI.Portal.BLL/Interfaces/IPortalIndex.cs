using System.Threading.Tasks;
using XI.Portal.BLL.ViewModels;

namespace XI.Portal.BLL.Interfaces
{
    public interface IPortalIndex
    {
        Task<PortalIndexViewModel> GetPortalIndexViewModel();
    }
}
