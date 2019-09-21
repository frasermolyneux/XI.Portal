using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnDamageEventArgs : ServerBaseEventArgs
    {
        public OnDamageEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}