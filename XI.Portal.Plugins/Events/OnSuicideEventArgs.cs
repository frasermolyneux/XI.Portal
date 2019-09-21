using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnSuicideEventArgs : ServerBaseEventArgs
    {
        public OnSuicideEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}