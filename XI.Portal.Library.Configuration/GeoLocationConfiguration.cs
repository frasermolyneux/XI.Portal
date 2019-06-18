using System;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class GeoLocationConfiguration
    {
        private readonly AppSettingConfigurationProvider appSettingConfigurationProvider;

        public GeoLocationConfiguration(AppSettingConfigurationProvider appSettingConfigurationProvider)
        {
            this.appSettingConfigurationProvider = appSettingConfigurationProvider ?? throw new ArgumentNullException(nameof(appSettingConfigurationProvider));
        }

        public string GeoLocationServiceUrl => appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.GeoLocationServiceUrl);
    }
}