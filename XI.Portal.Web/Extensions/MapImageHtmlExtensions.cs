using System.Web.Mvc;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Extensions
{
    public static class MapImageHtmlExtensions
    {
        public static MvcHtmlString GameTypeIcon(this HtmlHelper html, GameType gameType, string mapName)
        {
            return new MvcHtmlString(
                $"<img style=\"border: 5px solid #021a40; display: block; margin: auto;\" src=\"/MapImage/Image?gameType={gameType}&mapName={mapName}\" alt=\"{mapName}\" />");
        }
    }
}