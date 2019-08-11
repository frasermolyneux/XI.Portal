using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            ContextProvider = contextProvider;
            DatabaseLogger = databaseLogger;
        }

        protected IContextProvider ContextProvider { get; }
        protected IDatabaseLogger DatabaseLogger { get; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RequestContext.RouteData.Values.ContainsKey("Controller") ? filterContext.RequestContext.RouteData.Values["Controller"].ToString() : null;
            var action = filterContext.RequestContext.RouteData.Values.ContainsKey("Action") ? filterContext.RequestContext.RouteData.Values["Action"].ToString() : null;

            if (User.Identity.IsAuthenticated)
            {
                var userId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();
                DatabaseLogger.CreateUserLogAsync(userId, $"User loaded controller {controller} with action {action}");
            }
            else
            {
                DatabaseLogger.CreateUserLogAsync(null, $"Anon loaded controller {controller} with action {action}");
            }


                

            base.OnActionExecuting(filterContext);
        }
    }
}