using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.SyncService.BanFiles
{
    public interface IBanFileImporter
    {
        void ImportData(GameType gameType, string banData, Guid serverId);
    }
}