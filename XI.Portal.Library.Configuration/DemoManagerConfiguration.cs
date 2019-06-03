using System;
using Amazon;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class DemoManagerConfiguration
    {
        private readonly AppSettingConfigurationProvider appSettingConfigurationProvider;
        private readonly AwsConfiguration awsConfiguration;

        public DemoManagerConfiguration(AwsConfiguration awsConfiguration, AppSettingConfigurationProvider appSettingConfigurationProvider)
        {
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
            this.appSettingConfigurationProvider = appSettingConfigurationProvider ?? throw new ArgumentNullException(nameof(appSettingConfigurationProvider));
        }

        public RegionEndpoint AwsRegion => awsConfiguration.AwsRegion;
        public string AwsAccessKey => awsConfiguration.AwsAccessKey;

        public string AwsSecretKey => awsConfiguration.AwsSecretKey;

        public string DemoBucketName => appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.DemoManagerBucketName);
    }
}