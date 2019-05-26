using System.Collections.Generic;
using XI.Portal.Library.Rcon.Models;

namespace XI.Portal.Library.Rcon.Parser
{
    public interface IRconParser
    {
        List<RconPlayer> ParseData(string data, string gameName);
    }
}