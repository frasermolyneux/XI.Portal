using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Forums
{
    public class ForumsConfiguration : IForumsConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public ForumsConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string ApiKey => configurationProvider.GetConfigurationItem(ForumsConfigurationKeys.ApiKey);

        public string OAuthClientId => configurationProvider.GetConfigurationItem(ForumsConfigurationKeys.OAuthClientId);

        public string OAuthClientSecret => configurationProvider.GetConfigurationItem(ForumsConfigurationKeys.OAuthClientSecret);
    }
}
