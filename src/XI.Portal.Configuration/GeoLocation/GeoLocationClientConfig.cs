using System;
using System.Collections.Generic;
using FM.GeoLocation.Client;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.GeoLocation
{
    public class GeoLocationClientConfig : IGeoLocationClientConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public GeoLocationClientConfig(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public string BaseUrl => configurationProvider.GetConfigurationItem("FM.GeoLocation.BaseUrl");
        public string ApiKey => configurationProvider.GetConfigurationItem("FM.GeoLocation.ApiKey");

        public bool UseMemoryCache { get; } = true;
        public int CacheEntryLifeInMinutes { get; } = 60;

        public IEnumerable<TimeSpan> RetryTimespans
        {
            get
            {
                var random = new Random();

                return new[]
                {
                    TimeSpan.FromSeconds(random.Next(1)),
                    TimeSpan.FromSeconds(random.Next(3)),
                    TimeSpan.FromSeconds(random.Next(5))
                };
            }
        }
    }
}