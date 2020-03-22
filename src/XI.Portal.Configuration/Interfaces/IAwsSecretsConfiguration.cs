using Amazon;

namespace XI.Portal.Configuration.Interfaces
{
    public interface IAwsSecretsConfiguration
    {
        string AccessKey { get; }
        string SecretKey { get; }
        RegionEndpoint Region { get; }
        string SecretName { get; }
    }
}