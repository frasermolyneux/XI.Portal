using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class ServerBaseEventArgs : EventArgs
    {
        public ServerBaseEventArgs(Guid serverId, GameType gameType)
        {
            ServerId = serverId;
            GameType = gameType;
        }

        public Guid ServerId { get; }
        public GameType GameType { get; }
    }
}