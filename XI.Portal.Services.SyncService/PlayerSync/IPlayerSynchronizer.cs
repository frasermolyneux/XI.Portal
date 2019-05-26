using System;
using System.Collections.Generic;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Services.SyncService.Models;

namespace XI.Portal.Services.SyncService.PlayerSync
{
    public interface IPlayerSynchronizer
    {
        void SynchronizeLocalPlayers(GameType gameType, List<LocalPlayer> localPlayers, Guid serverId);
    }
}