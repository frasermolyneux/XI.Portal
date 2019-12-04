using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnActionEventArgs : ServerBaseEventArgs
    {
        public OnActionEventArgs(Guid serverId, string serverName, GameType gameType) : base(serverId, serverName, gameType)
        {
        }
    }
}