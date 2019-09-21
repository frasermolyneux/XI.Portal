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

        public override void ParseLine(string line, Guid serverId)
        {
            OnLineRead(new LineReadEventArgs
            {
                GameType = GameType.CallOfDuty4,
                LineData = line,
                ServerId = serverId
            });

            line = line.Replace("\r\n", "");
            line = line.Trim();
            line = line.Substring(line.IndexOf(' ') + 1);

            if (line.StartsWith("J;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];

                OnPlayerConnected(new OnPlayerConnectedEventArgs
                {
                    ServerId = serverId,
                    Guid = guid,
                    Name = name
                });
            }
            else if (line.StartsWith("L;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];

                OnPlayerDisconnected(new OnPlayerDisconnectedEventArgs
                {
                    ServerId = serverId,
                    Guid = guid,
                    Name = name
                });
            }
            else if (line.StartsWith("say;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];
                var message = parts[4];
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());

                OnChatMessage(new OnChatMessageEventArgs
                {
                    Guid = guid,
                    Name = name,
                    Message = message,
                    ServerId = serverId,
                    ChatType = ChatType.All
                });
            }
            else if (line.StartsWith("sayteam;"))
            {
                var parts = line.Split(';');
                var guid = parts[1];
                var name = parts[3];
                var message = parts[4];
                message = new string(message.Where(c => !char.IsControl(c)).ToArray());

                OnChatMessage(new OnChatMessageEventArgs
                {
                    Guid = guid,
                    Name = name,
                    Message = message,
                    ServerId = serverId,
                    ChatType = ChatType.Team
                });
            }
            else
            {
                Logger.Debug($"[{serverId}] {line}");
            }
        }
    }
}