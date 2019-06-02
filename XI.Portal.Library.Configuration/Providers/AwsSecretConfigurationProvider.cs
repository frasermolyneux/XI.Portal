using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace XI.Portal.Library.Configuration.Providers
{
    public class AwsSecretConfigurationProvider
    {
        private readonly AwsConfiguration awsConfiguration;

        public AwsSecretConfigurationProvider(AwsConfiguration awsConfiguration)
        {
            this.awsConfiguration = awsConfiguration ?? throw new ArgumentNullException(nameof(awsConfiguration));
        }

        public string GetConfigurationItem(string configurationKey)
        {
            var client = new AmazonSecretsManagerClient(new BasicAWSCredentials(awsConfiguration.AwsAccessKey, awsConfiguration.AwsSecretKey), awsConfiguration.AwsRegion);
            var secretValue = client.GetSecretValue(new GetSecretValueRequest {SecretId = awsConfiguration.AwsSecretName});

            var secrets = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretValue.SecretString);

            return secrets[configurationKey];
        }
    }
}