using System.Web.Mvc;

namespace XI.Portal.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static string IsSelected(this HtmlHelper html, string controller = null, string action = null,
            dynamic additionalValues = null, string cssClass = null)
        {
            if (string.IsNullOrEmpty(cssClass))
                cssClass = "active";

            var currentAction = ((string) html.ViewContext.RouteData.Values["action"]).ToLower();
            var currentController = ((string) html.ViewContext.RouteData.Values["controller"]).ToLower();
            var currentId = (string) html.ViewContext.RouteData.Values["id"];
            var currentGameId = (string) html.ViewContext.RouteData.Values["gameId"];
            var currentServerId = (string) html.ViewContext.RouteData.Values["serverId"];

            if (string.IsNullOrEmpty(controller))
                controller = currentController;

            if (string.IsNullOrEmpty(action))
                action = currentAction;

            string id;
            try
            {
                id = additionalValues?.id.ToString();
                if (string.IsNullOrEmpty(id))
                    id = currentId;
            }
            catch
            {
                id = currentId;
            }

            string serverId;
            try
            {
                serverId = additionalValues?.serverId;
                if (string.IsNullOrEmpty(serverId))
                    serverId = currentServerId;
            }
            catch
            {
                serverId = currentServerId;
            }

            string gameId;
            try
            {
                gameId = additionalValues?.gameId;
                if (string.IsNullOrEmpty(gameId))
                    gameId = currentGameId;
            }
            catch
            {
                gameId = currentGameId;
            }

            return controller == currentController && action == currentAction && id == currentId &&
                   serverId == currentServerId && gameId == currentGameId
                ? cssClass
                : string.Empty;
        }

        public static string PageClass(this HtmlHelper html)
        {
            var currentAction = (string) html.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
    }
}