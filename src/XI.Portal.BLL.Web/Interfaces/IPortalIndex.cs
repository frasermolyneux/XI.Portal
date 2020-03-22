using System.Threading.Tasks;
using XI.Portal.BLL.Web.ViewModels;

namespace XI.Portal.BLL.Web.Interfaces
{
    public interface IPortalIndex
    {
        Task<PortalIndexViewModel> GetPortalIndexViewModel();
    }
}
