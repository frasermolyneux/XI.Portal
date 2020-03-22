using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.GeoLocation
{
    public class GeoLocationConfiguration : IGeoLocationConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public GeoLocationConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string GeoLocationServiceUrl => configurationProvider.GetConfigurationItem(GeoLocationConfigurationKeys.GeoLocationServiceUrl);
    }
}
