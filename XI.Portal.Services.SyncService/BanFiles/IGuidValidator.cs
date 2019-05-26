using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.SyncService.BanFiles
{
    public interface IGuidValidator
    {
        bool IsValid(GameType gameType, string guid);
    }
}