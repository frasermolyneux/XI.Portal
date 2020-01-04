using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class ServerBaseEventArgs : MonitorBaseEventArgs
    {
        public ServerBaseEventArgs(Guid monitorId, Guid serverId, string serverName, GameType gameType) : base (monitorId)
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