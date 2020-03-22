using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Database
{
    public class DatabaseConfiguration : IDatabaseConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public DatabaseConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string PortalDbConnectionString => configurationProvider.GetConfigurationItem(DatabaseConfigurationKeys.PortalDbConnectionString);
    }
}
