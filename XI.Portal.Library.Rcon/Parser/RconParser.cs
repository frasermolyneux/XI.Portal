using System.Collections.Generic;
using System.Text.RegularExpressions;
using XI.Portal.Library.Rcon.Extensions;
using XI.Portal.Library.Rcon.Models;

namespace XI.Portal.Library.Rcon.Parser
{
    public class RconParser : IRconParser
    {
        public List<RconPlayer> ParseData(string data, string gameName)
        {
            var results = new List<RconPlayer>();

            var lines = data.Split('\n');
            foreach (var line in lines)
                try
                {
                    var match = Regex.Match(line, gameName.PlayerRconRegex());
                    if (match.Success)
                    {
                        var player = new RconPlayer
                        {
                            Num = match.Groups[1].ToString(),
                            Score = match.Groups[2].ToString(),
                            Ping = match.Groups[3].ToString(),
                            Guid = match.Groups[4].ToString(),
                            Name = match.Groups[5].ToString(),
                            LastMsg = match.Groups[6].ToString(),
                            IpAddress = match.Groups[7].ToString(),
                            Port = match.Groups[8].ToString(),
                            QueryPort = match.Groups[9].ToString(),
                            Rate = match.Groups[10].ToString()
                        };

                        if (player.IsValid) results.Add(player);
                    }
                }
                catch
                {
                    // ignored
                }

            return results;
        }
    }
}