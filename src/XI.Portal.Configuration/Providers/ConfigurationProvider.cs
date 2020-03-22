using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Providers
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly ILocalConfigurationProvider localConfigurationProvider;
        private readonly IAwsSecretConfigurationProvider awsSecretConfigurationProvider;

        public ConfigurationProvider(ILocalConfigurationProvider localConfigurationProvider, IAwsSecretConfigurationProvider awsSecretConfigurationProvider)
        {
            this.localConfigurationProvider = localConfigurationProvider ?? throw new System.ArgumentNullException(nameof(localConfigurationProvider));
            this.awsSecretConfigurationProvider = awsSecretConfigurationProvider ?? throw new System.ArgumentNullException(nameof(awsSecretConfigurationProvider));
        }

        public string GetConfigurationItem(string configurationKey)
        {
            return localConfigurationProvider.GetConfigurationItem(configurationKey) ?? awsSecretConfigurationProvider.GetConfigurationItem(configurationKey);
        }
    }
}
