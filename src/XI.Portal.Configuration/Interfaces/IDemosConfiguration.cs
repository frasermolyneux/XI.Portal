using Amazon;

namespace XI.Portal.Configuration.Interfaces
{
    public interface IDemosConfiguration
    {
        string AccessKey { get; }
        string SecretKey { get; }
        RegionEndpoint Region { get; }
        string S3BucketName { get; }
    }
}
