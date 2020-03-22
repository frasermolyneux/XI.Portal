using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Data.Contracts.FilterModels;

namespace XI.Portal.BLL.Web.Interfaces
{
    public interface IMapsList
    {
        Task<int> GetMapListCount(MapsFilterModel filterModel);
        Task<List<MapsListEntryViewModel>> GetMapList(MapsFilterModel filterModel);
    }
}
