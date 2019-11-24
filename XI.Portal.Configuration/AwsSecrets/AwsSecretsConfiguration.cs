using Amazon;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.AwsSecrets
{
    public class AwsSecretsConfiguration : IAwsSecretsConfiguration
    {
        private readonly ILocalConfigurationProvider appSettingConfigurationProvider;

        public AwsSecretsConfiguration(ILocalConfigurationProvider appSettingConfigurationProvider)
        {
            this.appSettingConfigurationProvider = appSettingConfigurationProvider 
                ?? throw new System.ArgumentNullException(nameof(appSettingConfigurationProvider));
        }

        public string AccessKey => appSettingConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.AccessKey);

        public string SecretKey => appSettingConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.SecretKey);

        public RegionEndpoint Region
        {
            get
            {
                var regionAsStr = appSettingConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.Region);
                return RegionEndpoint.GetBySystemName(regionAsStr);
            }
        }

        public string SecretName => appSettingConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.SecretName);
    }
}
