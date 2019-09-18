using System;
using System.IO;
using XI.Portal.Library.Configuration;
using XI.Portal.Services.FileMonitor.Events;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor.Plugins
{
    public class StatsLogProxyPlugin : IPlugin
    {
        private readonly StatsLogProxyPluginConfiguration statsLogProxyPluginConfiguration;

        public StatsLogProxyPlugin(StatsLogProxyPluginConfiguration statsLogProxyPluginConfiguration)
        {
            this.statsLogProxyPluginConfiguration = statsLogProxyPluginConfiguration ?? throw new ArgumentNullException(nameof(statsLogProxyPluginConfiguration));
        }

        public void RegisterEventHandlers(IParser parser)
        {
            parser.LineRead += Parser_LineRead;
        }

        private void Parser_LineRead(object sender, EventArgs e)
        {
            var lineReadEventArgs = (LineReadEventArgs) e;

            var localLogFilePath = $"{statsLogProxyPluginConfiguration.StatsLogBaseDirectory}\\{lineReadEventArgs.ServerId}\\games_mp.log";

            Directory.CreateDirectory(Path.GetDirectoryName(localLogFilePath));

            if (!File.Exists(localLogFilePath))
            {
                var file = File.Create(localLogFilePath);
                file.Close();
            }

            File.AppendAllText(localLogFilePath, $"{lineReadEventArgs.LineData}\n");
        }
    }
}