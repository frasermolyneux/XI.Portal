using System;
using System.IO;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Plugins.LogProxyPlugin
{
    public class LogProxyPlugin : IPlugin
    {
        private readonly ILogProxyPluginConfiguration statsLogProxyPluginConfiguration;

        public LogProxyPlugin(ILogProxyPluginConfiguration statsLogProxyPluginConfiguration)
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

            var commandsToBlock = new string[] { "!fu", "!like", "!dislike" };

            foreach (var command in commandsToBlock)
            {
                if (eventArgs.LineData.ToLower().Contains(command))
                {
                    return; // block commands from being proxied, will need to do this with any additional commands
                }
            }

            var localLogFilePath = $"{statsLogProxyPluginConfiguration.LogBaseDirectory}\\{eventArgs.ServerId}\\games_mp.log";

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