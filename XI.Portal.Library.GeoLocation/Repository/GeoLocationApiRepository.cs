using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Serilog;
using XI.Portal.Library.Configuration;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Repository
{
    public class GeoLocationApiRepository : IGeoLocationApiRepository
    {
        private readonly GeoLocationConfiguration geoLocationConfiguration;
        private readonly ILogger logger;

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
                var locationResult = await Policy.Handle<Exception>()
                    .WaitAndRetryAsync(GetRetryTimeSpans(), (result, timeSpan, retryCount, acontext) => { logger.Warning("Failed to get location for {address} - retry count: {count}", address, retryCount); })
                    .ExecuteAsync(async () => await GetLocationDto(address, encodedAddress));

                return locationResult;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get location for address {address}", address);
                return new LocationDto();
            }
        }

        private static IEnumerable<TimeSpan> GetRetryTimeSpans()
        {
            var random = new Random();

            return new[]
            {
                TimeSpan.FromSeconds(random.Next(1)),
                TimeSpan.FromSeconds(random.Next(3)),
                TimeSpan.FromSeconds(random.Next(5))
            };
        }

        private async Task<LocationDto> GetLocationDto(string address, string encodedAddress)
        {
            using (var wc = new WebClient())
            {
                var locationString = await wc.DownloadStringTaskAsync($"{geoLocationConfiguration.GeoLocationServiceUrl}/api/geo/location/{encodedAddress}");
                var deserializeLocation = JsonConvert.DeserializeObject<LocationDto>(locationString);

                logger.Debug("{@location} retrieved for {address}", deserializeLocation, address);

                return deserializeLocation;
            }
        }
    }
}