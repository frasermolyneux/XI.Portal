﻿using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnPlayerConnectedEventArgs : ServerBaseEventArgs
    {
        public OnPlayerConnectedEventArgs(Guid serverId, GameType gameType, string guid, string name) : base(serverId, gameType)
        {
            Guid = guid;
            Name = name;
        }

        public string Guid { get; }
        public string Name { get; }
    }
}