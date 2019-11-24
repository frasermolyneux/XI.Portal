using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Maps
{
    public class MapsConfiguration : IMapsConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public MapsConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string MapRedirectBaseUrl => configurationProvider.GetConfigurationItem(MapsConfigurationKeys.MapRedirectBaseUrl);

        public string MapRedirectKey => configurationProvider.GetConfigurationItem(MapsConfigurationKeys.MapRedirectKey);
    }
}
