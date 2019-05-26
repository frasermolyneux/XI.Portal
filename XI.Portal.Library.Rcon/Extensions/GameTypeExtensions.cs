using System;

namespace XI.Portal.Library.Rcon.Extensions
{
    public static class GameTypeExtensions
    {
        public static string PlayerRconRegex(this string gameName)
        {
            switch (gameName)
            {
                case "COD2":
                    return @"([0-9]{1,2})\s+([0-9]{1,4})\s+([0-9]{1,4})\s+([0-9]{5,})\s(.+)\^7\s+([0-9]{1,5})\s([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}):([0-9]{1,6})\s+([0-9-]{1,6})\s+([0-9]{1,5})";
                case "COD4":
                    return @"([0-9]{1,2})\s+([0-9]{1,4})\s+([0-9]{1,4})\s+([a-zA-Z0-9]{32})\s(.+)\^7\s+([0-9]{1,5})\s([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}):([0-9]{1,6})\s+([0-9-]{1,6})\s+([0-9]{1,5})";
                case "COD5":
                    return @"([0-9]{1,2})\s+([0-9]{1,4})\s+([0-9]{1,4})\s+([0-9]{5,})\s(.+)\^7\s+([0-9]{1,5})\s([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}):([0-9]{1,6})\s+([0-9-]{1,6})\s+([0-9]{1,5})";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}