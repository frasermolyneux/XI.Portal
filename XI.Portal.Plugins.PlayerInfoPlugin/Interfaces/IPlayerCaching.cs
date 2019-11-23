
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.PlayerInfoPlugin.Interfaces
{
    public interface IPlayerCaching
    {
        void AddToCache(GameType gameType, string guid, string name);
        bool PlayerInCache(GameType gameType, string guid, string name);
        void ReduceCache();
    }
}
