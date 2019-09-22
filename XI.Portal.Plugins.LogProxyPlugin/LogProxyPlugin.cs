using System;
using System.IO;
using XI.Portal.Library.Configuration;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Plugins.LogProxyPlugin
{
    public class LogProxyPlugin : IPlugin
    {
        private readonly StatsLogProxyPluginConfiguration statsLogProxyPluginConfiguration;

        public LogProxyPlugin(StatsLogProxyPluginConfiguration statsLogProxyPluginConfiguration)
        {
            this.statsLogProxyPluginConfiguration = statsLogProxyPluginConfiguration ?? throw new ArgumentNullException(nameof(statsLogProxyPluginConfiguration));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.LineRead += Parser_LineRead;
        }

        private void Parser_LineRead(object sender, EventArgs e)
        {
            var eventArgs = (LineReadEventArgs) e;

            if (eventArgs.LineData.ToLower().Contains("!fu")) return; // block !fu command from being proxied, will need to do this with any additional commands

            var localLogFilePath = $"{statsLogProxyPluginConfiguration.StatsLogBaseDirectory}\\{eventArgs.ServerId}\\games_mp.log";

            Directory.CreateDirectory(Path.GetDirectoryName(localLogFilePath));

            if (!File.Exists(localLogFilePath))
            {
                var file = File.Create(localLogFilePath);
                file.Close();
            }

            File.AppendAllText(localLogFilePath, $"{eventArgs.LineData}\n");
        }
    }
}