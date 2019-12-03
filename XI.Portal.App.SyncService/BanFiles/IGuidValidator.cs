using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanFiles
{
    public interface IGuidValidator
    {
        bool IsValid(GameType gameType, string guid);
    }
}