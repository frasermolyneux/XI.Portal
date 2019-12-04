﻿using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnTeamKillEventArgs : ServerBaseEventArgs
    {
        public OnTeamKillEventArgs(Guid serverId, string serverName, GameType gameType) : base(serverId, serverName, gameType)
        {
        }
    }
}