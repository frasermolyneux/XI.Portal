using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Repository
{
    public class GeoLocationApiRepository : IGeoLocationApiRepository
    {
        private readonly ILogger logger;
        private readonly GeoLocationConfiguration geoLocationConfiguration;

        public GeoLocationApiRepository(ILogger logger, GeoLocationConfiguration geoLocationConfiguration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                    var deserializeLocation = JsonConvert.DeserializeObject<LocationDto>(locationString);

                    logger.Debug($"Location {deserializeLocation} retrieved for {address}");

                    return deserializeLocation;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Failed to get location for address {address}");

                return new LocationDto();
            }
        }
    }
}