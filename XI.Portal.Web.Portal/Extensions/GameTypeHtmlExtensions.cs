using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Portal.Extensions
{
    public static class GameTypeHtmlExtensions
    {
        public static MvcHtmlString GameTypeIcon(this HtmlHelper html, GameType gameType)
        {
            return new MvcHtmlString($"<img src=\"/Images/icons/{gameType}.png\" alt=\"{gameType}\" width=\"16\" height=\"16\" />");
        }

        public static MvcHtmlString GameTypeIconExternal(this HtmlHelper html, GameType gameType)
        {
            return new MvcHtmlString($"<img src=\"https://portal.xtremeidiots.com/Images/icons/{gameType}.png\" alt=\"{gameType}\" width=\"16\" height=\"16\" />");
        }

        public static string GameRedirectBaseUrl(this GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CallOfDuty4:
                    return "https://redirect.xtremeidiots.net/redirect/cod4/usermaps/";
                case GameType.CallOfDuty5:
                    return "https://redirect.xtremeidiots.net/redirect/cod5/usermaps/";
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
            }
        }

        public static IEnumerable<SelectListItem> DemoManagerSupportedGames(this HtmlHelper html)
        {
            var supportedGameTypes = new List<GameType>
            {
                GameType.CallOfDuty2,
                GameType.CallOfDuty4,
                GameType.CallOfDuty5
            };

            return supportedGameTypes
                .Select(e => new SelectListItem
                {
                    Value = ((int) e).ToString(),
                    Text = e.DisplayName()
                });
        }
    }
}