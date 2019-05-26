using System.Collections.Generic;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Portal.Extensions
{
    public static class ServerHtmlExtensions
    {
        public static string ServerName(this HtmlHelper html, GameServer server)
        {
            var toRemove = new List<string> {"^1", "^2", "^3", "^4", "^5", "^6", "^7", "^8", "^9"};

            if (string.IsNullOrWhiteSpace(server.LiveTitle)) return server.Title;

            var serverName = server.LiveTitle;
            foreach (var val in toRemove) serverName = serverName.Replace(val, "");
            return serverName;
        }

        public static string ServerHostAndPort(this HtmlHelper html, string hostname, int port)
        {
            if (string.IsNullOrWhiteSpace(hostname) && port == 0) return "";

            if (!string.IsNullOrWhiteSpace(hostname) && port == 0) return hostname;

            return $"{hostname}:{port}";
        }

        public static MvcHtmlString GameTrackerIcon(this HtmlHelper html, string hostname, int port)
        {
            var link = $"https://www.gametracker.com/server_info/{hostname}:{port}";
            return new MvcHtmlString(
                $"<a style=\"margin:5px\" href=\"{link}\" target=\"_blank\"><img src=\"/Images/icons/gametracker.png\" alt=\"gametracker\"/></a>");
        }

        public static MvcHtmlString HlswIcon(this HtmlHelper html, GameType gameType, string hostname, int port)
        {
            var link = "";
            switch (gameType)
            {
                case GameType.CallOfDuty2:
                    link = $"hlsw://{hostname}:{port}?Game=CoD2";
                    break;
                case GameType.CallOfDuty4:
                    link = $"hlsw://{hostname}:{port}?Game=CoD4";
                    break;
                case GameType.CallOfDuty5:
                    link = $"hlsw://{hostname}:{port}?Game=CoDWW";
                    break;
                default:
                    return new MvcHtmlString("");
            }

            return new MvcHtmlString(
                $"<a style=\"margin:5px\" href=\"{link}\"><img src=\"/Images/icons/hlsw.png\" alt=\"hlsw\"/></a>");
        }

        public static MvcHtmlString SteamIcon(this HtmlHelper html, GameType gameType, string hostname, int port)
        {
            var link = $"steam://connect/{hostname}:{port}";
            return new MvcHtmlString(
                $"<a style=\"margin:5px\" href=\"{link}\"><img src=\"/Images/icons/steam.png\" alt=\"steam\"/></a>");
        }
    }
}