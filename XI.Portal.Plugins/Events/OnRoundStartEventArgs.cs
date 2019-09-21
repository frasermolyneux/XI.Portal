using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnRoundStartEventArgs : ServerBaseEventArgs
    {
        public OnRoundStartEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}