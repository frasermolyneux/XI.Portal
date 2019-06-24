using System;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class MapRedirectConfiguration
    {
        private readonly AppSettingConfigurationProvider appSettingConfigurationProvider;
        private readonly AwsSecretConfigurationProvider awsSecretConfigurationProvider;

        public MapRedirectConfiguration(AppSettingConfigurationProvider appSettingConfigurationProvider, AwsSecretConfigurationProvider awsSecretConfigurationProvider)
        {
            this.appSettingConfigurationProvider = appSettingConfigurationProvider ?? throw new ArgumentNullException(nameof(appSettingConfigurationProvider));
            this.awsSecretConfigurationProvider = awsSecretConfigurationProvider ?? throw new ArgumentNullException(nameof(awsSecretConfigurationProvider));
        }

        public string MapRedirectBaseUrl => appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.MapRedirectBaseUrl);

        public string MapRedirectKey => appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.MapRedirectKey);
    }
}