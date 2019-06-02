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
#if DEBUG
                var regionAsStr = Environment.GetEnvironmentVariable(ConfigurationKeys.AwsRegion);
#else
                var regionAsStr = appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsRegion);
#endif
                return RegionEndpoint.GetBySystemName(regionAsStr);
            }
        }

        public string AwsAccessKey
        {
            get
            {
#if DEBUG
                return Environment.GetEnvironmentVariable(ConfigurationKeys.AwsAccessKey);
#else
                return appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsAccessKey);
#endif
            }
        }

        public string AwsSecretKey
        {
            get
            {
#if DEBUG
                return Environment.GetEnvironmentVariable(ConfigurationKeys.AwsSecretKey);
#else
                return appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsSecretKey);
#endif
            }
        }

        public string AwsSecretName
        {
            get
            {
#if DEBUG
                return Environment.GetEnvironmentVariable(ConfigurationKeys.AwsPortalSecretName);
#else
                return appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.AwsPortalSecretName);
#endif
            }
        }
    }
}