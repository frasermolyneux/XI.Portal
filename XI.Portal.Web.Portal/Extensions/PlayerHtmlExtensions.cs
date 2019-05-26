using System.Collections.Generic;
using System.Web.Mvc;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Portal.Extensions
{
    public static class PlayerHtmlExtensions
    {
        public static string PlayerName(this HtmlHelper html, string playerName)
        {
            var toRemove = new List<string> {"^1", "^2", "^3", "^4", "^5", "^6", "^7", "^8", "^9"};
            foreach (var val in toRemove) playerName = playerName.Replace(val, "");

            return playerName;
        }

        public static MvcHtmlString GuidLink(this HtmlHelper html, string guid, GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CallOfDuty4:
                    var link = $"https://www.pbbans.com/mbi.php?action=12&guid={guid}";
                    return new MvcHtmlString(
                        $"<a style=\"margin:5px\" href=\"{link}\" target=\"_blank\">{guid}</a>");
                default:
                    return new MvcHtmlString(guid);
            }
        }
    }
}