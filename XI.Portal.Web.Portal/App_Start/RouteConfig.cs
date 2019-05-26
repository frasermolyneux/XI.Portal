using System.Web.Mvc;
using System.Web.Routing;

namespace XI.Portal.Web.Portal
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Landing", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}