using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.SyncService.BanFiles
{
    public interface IBanFileImporter
    {
        void ImportData(GameType gameType, string banData, Guid serverId);
    }
}