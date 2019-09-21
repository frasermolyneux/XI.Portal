using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnKillEventArgs : ServerBaseEventArgs
    {
        public OnKillEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}