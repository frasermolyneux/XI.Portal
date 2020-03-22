using System;
using System.Configuration;
using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.Providers
{

    public class LocalConfigurationProvider : ILocalConfigurationProvider
    {
        public string GetConfigurationItem(string configurationKey)
        {
            return Environment.GetEnvironmentVariable(configurationKey) ?? ConfigurationManager.AppSettings[configurationKey];
        }
    }
}
