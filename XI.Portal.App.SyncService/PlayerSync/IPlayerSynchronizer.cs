using System;
using System.Collections.Generic;
using XI.Portal.App.SyncService.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.PlayerSync
{
    public interface IPlayerSynchronizer
    {
        void SynchronizeLocalPlayers(GameType gameType, List<LocalPlayer> localPlayers, Guid serverId);
    }
}