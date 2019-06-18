using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Repository
{
    public class GeoLocationApiRepository : IGeoLocationApiRepository
    {
        private readonly GeoLocationConfiguration geoLocationConfiguration;

        public GeoLocationApiRepository(GeoLocationConfiguration geoLocationConfiguration)
        {
            this.geoLocationConfiguration = geoLocationConfiguration ?? throw new ArgumentNullException(nameof(geoLocationConfiguration));
        }

        public async Task<LocationDto> GetLocation(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return new LocationDto();

            var encodedAddress = Convert.ToBase64String(Encoding.UTF8.GetBytes(address));

            try
            {
                using (var wc = new WebClient())
                {
                    var locationString = await wc.DownloadStringTaskAsync($"{geoLocationConfiguration.GeoLocationServiceUrl}/api/geo/location/{encodedAddress}");
                    return JsonConvert.DeserializeObject<LocationDto>(locationString);
                }
            }
            catch
            {
                return new LocationDto();
            }
        }
    }
}