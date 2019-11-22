﻿using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnPlayerDisconnectedEventArgs : ServerBaseEventArgs
    {
        public OnPlayerDisconnectedEventArgs(Guid serverId, string serverName, GameType gameType, string guid, string name) : base(serverId, serverName, gameType)
        {
            Guid = guid;
            Name = name;
        }

        public string Guid { get; }
        public string Name { get; }
    }
}