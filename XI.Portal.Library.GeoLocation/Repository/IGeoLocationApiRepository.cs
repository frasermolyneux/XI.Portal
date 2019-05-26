using System.Threading.Tasks;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Repository
{
    public interface IGeoLocationApiRepository
    {
        Task<LocationDto> GetLocation(string address);
    }
}