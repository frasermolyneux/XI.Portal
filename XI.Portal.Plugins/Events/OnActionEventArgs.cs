using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnActionEventArgs : ServerBaseEventArgs
    {
        public OnActionEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}