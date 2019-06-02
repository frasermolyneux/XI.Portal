using System;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class XtremeIdiotsForumsConfiguration
    {
        private readonly AwsSecretConfigurationProvider awsSecretConfigurationProvider;

        public XtremeIdiotsForumsConfiguration(AwsSecretConfigurationProvider awsSecretConfigurationProvider)
        {
            this.awsSecretConfigurationProvider = awsSecretConfigurationProvider ?? throw new ArgumentNullException(nameof(awsSecretConfigurationProvider));
        }

        public string XtremeIdiotsForumsApiKey => awsSecretConfigurationProvider.GetConfigurationItem(ConfigurationKeys.XtremeIdiotsForumsApiKey);
        public string XtremeIdiotsOAuthClientId => awsSecretConfigurationProvider.GetConfigurationItem(ConfigurationKeys.XtremeIdiotsOAuthClientId);
        public string XtremeIdiotsOAuthClientSecret => awsSecretConfigurationProvider.GetConfigurationItem(ConfigurationKeys.XtremeIdiotsOAuthClientSecret);
    }
}