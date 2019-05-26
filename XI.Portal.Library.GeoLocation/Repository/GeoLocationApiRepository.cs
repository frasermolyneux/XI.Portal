using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Library.GeoLocation.Repository
{
    public class GeoLocationApiRepository : IGeoLocationApiRepository
    {
        private static string GeoLocationUrlService => ConfigurationManager.AppSettings["GeoLocationServiceUrl"];

        public async Task<LocationDto> GetLocation(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return new LocationDto();

            var encodedAddress = Convert.ToBase64String(Encoding.UTF8.GetBytes(address));

            try
            {
                using (var wc = new WebClient())
                {
                    var locationString = await wc.DownloadStringTaskAsync($"{GeoLocationUrlService}/{encodedAddress}");
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