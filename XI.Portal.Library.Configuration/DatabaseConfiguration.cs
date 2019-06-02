using System;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class DatabaseConfiguration
    {
        private readonly AwsSecretConfigurationProvider awsSecretConfigurationProvider;

        public DatabaseConfiguration(AwsSecretConfigurationProvider awsSecretConfigurationProvider)
        {
            this.awsSecretConfigurationProvider = awsSecretConfigurationProvider ?? throw new ArgumentNullException(nameof(awsSecretConfigurationProvider));
        }

        public string DbConnectionString => awsSecretConfigurationProvider.GetConfigurationItem(ConfigurationKeys.DbConnectionString);
    }
}