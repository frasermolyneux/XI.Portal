using System.Configuration;

namespace XI.Portal.Library.Configuration.Providers
{
    public class AppSettingConfigurationProvider
    {
        public string GetConfigurationItem(string configurationKey)
        {
            return ConfigurationManager.AppSettings[configurationKey];
        }
    }
}