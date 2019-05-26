using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Library.GameTracker
{
    public interface IMapImageRepository
    {
        string GetMapFilePath(GameType gameType, string mapName);
    }
}