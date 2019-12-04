using System;
using System.Collections.Generic;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Library.Rcon.Interfaces
{
    public interface IRconClientFactory
    {
        IRconClient CreateInstance(GameType gameType, string serverName, string hostname, int queryPort, string rconPassword);
        IRconClient CreateInstance(GameType gameType, string title, string hostname, int queryPort, string rconPassword, List<TimeSpan> retryOverride);
    }
}