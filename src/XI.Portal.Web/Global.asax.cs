using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var lastException = Server.GetLastError();

            var logger = DependencyResolver.Current.GetService<IDatabaseLogger>();
            logger.CreateSystemLogAsync("Error", "[Portal] Unhandled Error", lastException.Message);
        }
    }
}