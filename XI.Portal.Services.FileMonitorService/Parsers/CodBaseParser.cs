using System;
using System.Linq;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.Events;

namespace XI.Portal.Services.FileMonitorService.Parsers
{
    public class CodBaseParser : BaseParser
    {
        public CodBaseParser(ILogger logger) : base(logger)
        {
        }

        public override void ParseLine(string line, Guid serverId, string serverName, GameType gameType)
        {
            OnLineRead(new LineReadEventArgs(serverId, serverName, gameType, line));

            line = line.Replace("\r\n", "");
            line = line.Trim();
            line = line.Substring(line.IndexOf(' ') + 1);

            if (line.StartsWith("J;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];

                OnPlayerConnected(new OnPlayerConnectedEventArgs(serverId, serverName, gameType, guid, name));
            }
            else if (line.StartsWith("Q;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];

                OnPlayerDisconnected(new OnPlayerDisconnectedEventArgs(serverId, serverName, gameType, guid, name));
            }
            else if (line.StartsWith("say;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];
                var message = parts[4];
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());

                OnChatMessage(new OnChatMessageEventArgs(serverId, serverName, gameType, guid, name, message, ChatType.All));
            }
            else if (line.StartsWith("sayteam;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];
                var message = parts[4];
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());

                OnChatMessage(new OnChatMessageEventArgs(serverId, serverName, gameType, guid, name, message, ChatType.Team));
            }
            else
            {
                //Logger.Debug("[{serverName}] {line}", serverName, line);
            }
        }
    }
}