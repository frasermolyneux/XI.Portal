using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XI.Portal.Library.Configuration.Providers;

namespace XI.Portal.Library.Configuration
{
    public class StatsLogProxyPluginConfiguration
    {
        private readonly AppSettingConfigurationProvider appSettingConfigurationProvider;

        public StatsLogProxyPluginConfiguration(AppSettingConfigurationProvider appSettingConfigurationProvider)
        {
            this.appSettingConfigurationProvider = appSettingConfigurationProvider ?? throw new ArgumentNullException(nameof(appSettingConfigurationProvider));
        }

        public string StatsLogBaseDirectory => appSettingConfigurationProvider.GetConfigurationItem(ConfigurationKeys.StatsLogBaseDirectory);
    }
}
