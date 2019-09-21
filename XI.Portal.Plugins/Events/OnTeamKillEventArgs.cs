using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnTeamKillEventArgs : ServerBaseEventArgs
    {
        public OnTeamKillEventArgs(Guid serverId, GameType gameType) : base(serverId, gameType)
        {
        }
    }
}