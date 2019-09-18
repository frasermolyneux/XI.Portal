using System;
using System.Linq;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Rcon.Client;
using XI.Portal.Services.FileMonitor.Events;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor.Plugins
{
    internal class FuckYouPlugin : IPlugin
    {
        private readonly ContextProvider contextProvider;
        private readonly ILogger logger;

        public FuckYouPlugin(ILogger logger, ContextProvider contextProvider)
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

            if (onChatMessageEventArgs.Name.Contains(">XI<") && onChatMessageEventArgs.Message.Contains("!fu"))
            {
                logger.Information($"FuckYou initiated for {onChatMessageEventArgs.Name} ({onChatMessageEventArgs.Guid}) on server {onChatMessageEventArgs.ServerId}");

                using (var context = contextProvider.GetContext())
                {
                    var server = context.GameServers.Single(s => s.ServerId == onChatMessageEventArgs.ServerId);

                    var rconClient = new RconClient(server.Hostname, server.QueryPort, server.RconPassword);

                    string[] authors =
                    {
                        "Mahesh Chand", "Jeff Prosise", "Dave McCarter", "Allen O'neill",
                        "Monica Rathbun", "Henry He", "Raj Kumar", "Mark Prime",
                        "Rose Tracey", "Mike Crown"
                    };

                    var rand = new Random();
                    var index = rand.Next(authors.Length);

                    var responseMessage = $"{onChatMessageEventArgs.Name} likes {authors[index]}";
                    rconClient.SayCommand(responseMessage);
                    logger.Information($"FuckYou: {responseMessage}");
                }
            }
        }
    }
}