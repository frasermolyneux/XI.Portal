using System.Web.Mvc;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Extensions
{
    public static class MapImageHtmlExtensions
    {
        public static MvcHtmlString MapImage(this HtmlHelper html, GameType gameType, string mapName)
        {
            return new MvcHtmlString(
                $"<img style=\"border: 5px solid #021a40; display: block; margin: auto;\" src=\"/MapImage/Image?gameType={gameType}&mapName={mapName}\" alt=\"{mapName}\" />");
        }

        public static MvcHtmlString MapPopularity(this HtmlHelper html, string mapName, double likePercentage, double dislikePercentage, double totalLikes, double totalDislikes, int totalVotes)
        {
            return new MvcHtmlString(
                $"<div class=\"progress\" id=\"progress-{mapName}\">" +
                    $"<div class=\"progress-bar bg-info\" role=\"progressbar\" style=\"width: {likePercentage}%\" aria-valuenow=\"{totalLikes}\" aria-valuemin=\"0\" aria-valuemax=\"{totalVotes}\"></div>" +
                    $"<div class=\"progress-bar bg-danger\" role=\"progressbar\" style=\"width: {dislikePercentage}%\" aria-valuenow=\"{totalDislikes}\" aria-valuemin=\"0\" aria-valuemax=\"{totalVotes}\"></div>" +
                        "</div>" +
                $"<div class=\"m-t-sm\">{totalLikes} likes and {totalDislikes} dislikes out of {totalVotes}</div>");
        }
    }
}