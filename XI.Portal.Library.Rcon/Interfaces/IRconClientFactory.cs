using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Library.Rcon.Interfaces
{
    public interface IRconClientFactory
    {
        IRconClient CreateInstance(GameType gameType, string serverName, string hostname, int queryPort, string rconPassword);
    }
}