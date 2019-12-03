using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class ServerBaseEventArgs : EventArgs
    {
        public ServerBaseEventArgs(Guid serverId, string serverName, GameType gameType)
        {
            ServerId = serverId;
            ServerName = serverName;
            GameType = gameType;
        }

        public Guid ServerId { get; }
        public string ServerName { get; }
        public GameType GameType { get; }
    }
}