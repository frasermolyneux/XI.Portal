using Amazon;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.AwsSecrets
{
    public class AwsSecretsConfiguration : IAwsSecretsConfiguration
    {
        private readonly ILocalConfigurationProvider localConfigurationProvider;

        public AwsSecretsConfiguration(ILocalConfigurationProvider localConfigurationProvider)
        {
            this.localConfigurationProvider = localConfigurationProvider 
                ?? throw new System.ArgumentNullException(nameof(localConfigurationProvider));
        }

        public string AccessKey => localConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.AccessKey);

        public string SecretKey => localConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.SecretKey);

        public RegionEndpoint Region
        {
            get
            {
                var regionAsStr = localConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.Region);
                return RegionEndpoint.GetBySystemName(regionAsStr);
            }
        }

        public string SecretName => localConfigurationProvider.GetConfigurationItem(AwsSecretsConfigurationKeys.SecretName);
    }
}
