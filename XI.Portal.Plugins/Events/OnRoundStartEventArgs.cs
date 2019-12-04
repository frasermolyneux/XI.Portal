using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnRoundStartEventArgs : ServerBaseEventArgs
    {
        public OnRoundStartEventArgs(Guid serverId, string serverName, GameType gameType) : base(serverId, serverName, gameType)
        {
        }
    }
}