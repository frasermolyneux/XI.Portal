using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnSuicideEventArgs : ServerBaseEventArgs
    {
        public OnSuicideEventArgs(Guid serverId, string serverName, GameType gameType) : base(serverId, serverName, gameType)
        {
        }
    }
}