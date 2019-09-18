using System;
using Amazon;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class AwsConfiguration
    {
        private readonly AppSettingConfigurationProvider appSettingConfigurationProvider;

        public AwsConfiguration(AppSettingConfigurationProvider appSettingConfigurationProvider)
        {
            this.appSettingConfigurationProvider = appSettingConfigurationProvider ?? throw new ArgumentNullException(nameof(appSettingConfigurationProvider));
        }

        public RegionEndpoint AwsRegion
        {
            get
            {
                var regionAsStr = Environment.GetEnvironmentVariable(ConfigurationKeys.AwsRegion) ?? appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsRegion);
                return RegionEndpoint.GetBySystemName(regionAsStr);
            }
        }

        public string AwsAccessKey => Environment.GetEnvironmentVariable(ConfigurationKeys.AwsAccessKey) ?? appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsAccessKey);

        public string AwsSecretKey => Environment.GetEnvironmentVariable(ConfigurationKeys.AwsSecretKey) ?? appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsSecretKey);

        public string AwsSecretName => Environment.GetEnvironmentVariable(ConfigurationKeys.AwsPortalSecretName) ?? appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsPortalSecretName);
    }
}