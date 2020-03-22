using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Services.GameTracker.Interfaces
{
    public interface IMapImageRepository
    {
        string GetMapFilePath(GameType gameType, string mapName);
    }
}