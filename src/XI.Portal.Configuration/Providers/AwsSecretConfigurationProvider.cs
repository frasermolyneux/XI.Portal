using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using XI.Portal.Configuration.Cache;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Providers
{
    public class AwsSecretConfigurationProvider : IAwsSecretConfigurationProvider
    {
        private readonly IAwsSecretsConfiguration awsSecretsConfiguration;

        public AwsSecretConfigurationProvider(IAwsSecretsConfiguration awsSecretsConfiguration)
        {
            this.awsSecretsConfiguration = awsSecretsConfiguration ??
                                           throw new ArgumentNullException(nameof(awsSecretsConfiguration));
        }

        public string GetConfigurationItem(string configurationKey)
        {
            var client = new AmazonSecretsManagerClient(
                new BasicAWSCredentials(awsSecretsConfiguration.AccessKey, awsSecretsConfiguration.SecretKey),
                awsSecretsConfiguration.Region);

            if (AwsSecretsCache.CachedSecrets.ContainsKey(configurationKey) && AwsSecretsCache.Cached >= DateTime.UtcNow.AddMinutes(-15))
                return AwsSecretsCache.CachedSecrets[configurationKey];

            var secretValue = client.GetSecretValueAsync(new GetSecretValueRequest
                {SecretId = awsSecretsConfiguration.SecretName}).Result;
            var secrets = JsonConvert.DeserializeObject<Dictionary<string, string>>(secretValue.SecretString);

            AwsSecretsCache.CachedSecrets = secrets;
            AwsSecretsCache.Cached = DateTime.UtcNow;

            return AwsSecretsCache.CachedSecrets.ContainsKey(configurationKey) ? AwsSecretsCache.CachedSecrets[configurationKey] : null;
        }
    }
}