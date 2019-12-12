using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Contracts.Repositories
{
    public interface IMapsRepository
    {
        Task<int> GetMapCount(MapsFilterModel filterModel);
        Task<List<Map>> GetMaps(MapsFilterModel filterModel);
    }
}
