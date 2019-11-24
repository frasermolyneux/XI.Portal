using XI.Portal.Configuration.Interfaces;

namespace XI.Portal.Configuration.LogProxyPlugin
{
    public class LogProxyPluginConfiguration : ILogProxyPluginConfiguration
    {
        private readonly IConfigurationProvider configurationProvider;

        public LogProxyPluginConfiguration(IConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider ?? throw new System.ArgumentNullException(nameof(configurationProvider));
        }

        public string LogBaseDirectory => configurationProvider.GetConfigurationItem(LogProxyPluginConfigurationKeys.LogBaseDirectory);
    }
}
