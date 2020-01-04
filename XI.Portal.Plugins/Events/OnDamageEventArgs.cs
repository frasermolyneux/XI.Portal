using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnDamageEventArgs : ServerBaseEventArgs
    {
        public OnDamageEventArgs(Guid monitorId, Guid serverId, string serverName, GameType gameType) : base(monitorId, serverId, serverName, gameType)
        {
        }
    }
}