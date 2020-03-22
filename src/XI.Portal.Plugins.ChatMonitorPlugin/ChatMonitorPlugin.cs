using System;
using System.Linq;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Plugins.ChatMonitorPlugin
{
    public class ChatMonitorPlugin : IPlugin
    {
        private readonly IContextProvider contextProvider;
        private readonly ILogger logger;

        public ChatMonitorPlugin(ILogger logger, IContextProvider contextProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.ChatMessage += Parser_ChatMessage;
        }

        private void Parser_ChatMessage(object sender, EventArgs e)
        {
            var eventArgs = (OnChatMessageEventArgs) e;

            logger.Information("[{serverName}] [{chatType}] {name}: {message}", eventArgs.ServerName, eventArgs.ChatType, eventArgs.Name, eventArgs.Message);

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == eventArgs.ServerId);

                var player = context.Players.SingleOrDefault(p => p.Guid == eventArgs.Guid && p.GameType == gameServer.GameType);

                if (player == null)
                {
                    logger.Debug("[{serverName}] {name} does not exist in database - message will not be saved", eventArgs.ServerName, eventArgs.Name);
                    return;
                }

                var chatLog = new ChatLog
                {
                    GameServer = gameServer,
                    Player = player,
                    Username = eventArgs.Name,
                    ChatType = eventArgs.ChatType,
                    Message = eventArgs.Message,
                    Timestamp = DateTime.UtcNow
                };

                context.ChatLogs.Add(chatLog);
                context.SaveChanges();
            }
        }
    }
}