using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Providers
{

    public class AwsSecretConfigurationProvider : IAwsSecretConfigurationProvider
    {
        private readonly IAwsSecretsConfiguration awsSecretsConfiguration;

        public AwsSecretConfigurationProvider(IAwsSecretsConfiguration awsSecretsConfiguration)
        {
            this.awsSecretsConfiguration = awsSecretsConfiguration ?? throw new System.ArgumentNullException(nameof(awsSecretsConfiguration));
        }

        public string GetConfigurationItem(string configurationKey)
        {
            var client = new AmazonSecretsManagerClient(new BasicAWSCredentials(awsSecretsConfiguration.AccessKey, awsSecretsConfiguration.SecretKey), awsSecretsConfiguration.Region);
            var secretValue = client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = awsSecretsConfiguration.SecretName }).Result;

            var secrets = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretValue.SecretString);

            return secrets[configurationKey];
        }
    }
}
