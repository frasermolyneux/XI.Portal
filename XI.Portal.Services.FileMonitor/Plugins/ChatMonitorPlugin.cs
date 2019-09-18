using System;
using System.Linq;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Services.FileMonitor.Events;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor.Plugins
{
    internal class ChatMonitorPlugin : IPlugin
    {
        private readonly IContextProvider contextProvider;
        private readonly ILogger logger;

        public ChatMonitorPlugin(ILogger logger, IContextProvider contextProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void RegisterEventHandlers(IParser parser)
        {
            parser.ChatMessage += Parser_ChatMessage;
        }

        private void Parser_ChatMessage(object sender, EventArgs e)
        {
            var onChatMessageEventArgs = (OnChatMessageEventArgs) e;

            logger.Information($"[{onChatMessageEventArgs.ServerId}] [{onChatMessageEventArgs.ChatType}] {onChatMessageEventArgs.Name}: {onChatMessageEventArgs.Message}");

            using (var context = contextProvider.GetContext())
            {
                var gameServer = context.GameServers.Single(s => s.ServerId == onChatMessageEventArgs.ServerId);

                var player = context.Players.SingleOrDefault(p => p.Guid == onChatMessageEventArgs.Guid && p.GameType == gameServer.GameType);

                if (player == null)
                {
                    logger.Debug("Player does not exist in database - message will not be saved");
                    return;
                }

                var chatLog = new ChatLog
                {
                    GameServer = gameServer,
                    Player = player,
                    Username = onChatMessageEventArgs.Name,
                    ChatType = onChatMessageEventArgs.ChatType,
                    Message = onChatMessageEventArgs.Message,
                    Timestamp = DateTime.UtcNow
                };

                context.ChatLogs.Add(chatLog);
                context.SaveChanges();
            }
        }
    }
}