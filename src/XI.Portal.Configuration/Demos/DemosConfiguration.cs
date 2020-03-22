using Amazon;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Demos
{
    public class DemosConfiguration : IDemosConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public DemosConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string AccessKey => configurationProvider.GetConfigurationItem(DemosConfigurationKeys.AccessKey);

        public string SecretKey => configurationProvider.GetConfigurationItem(DemosConfigurationKeys.SecretKey);

        public RegionEndpoint Region
        {
            get
            {
                var regionAsStr = configurationProvider.GetConfigurationItem(DemosConfigurationKeys.Region);
                return RegionEndpoint.GetBySystemName(regionAsStr);
            }
        }

        public string S3BucketName => configurationProvider.GetConfigurationItem(DemosConfigurationKeys.S3BucketName);
    }
}
