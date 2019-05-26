using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.SyncService.BanFiles
{
    public interface ILocalBanFileManager
    {
        bool UpToDateBanFileExists(GameType gameType);
        void GenerateBanFileIfRequired(GameType gameType);
        long GetLocalBanFileSize(GameType gameType);
        void GenerateBanFile(GameType gameType);
    }
}