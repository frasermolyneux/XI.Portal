using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
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
            try
            {
                var action = filterContext.RequestContext.RouteData.Values.ContainsKey("Action") ? filterContext.RequestContext.RouteData.Values["Action"].ToString() : null;

                var paramsAsDictionary = filterContext.RequestContext.RouteData.Values
                    .Select(r => new { r.Key, r.Value })
                    .ToDictionary(i => i.Key, i => i.Value);

                var parameters = JsonConvert.SerializeObject(paramsAsDictionary);

                var ignoreActions = new[] { "GameServerList", "MapImage" };

                if (!ignoreActions.Contains(action))
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var userId = filterContext.RequestContext.HttpContext.User.Identity.GetUserId();
                        DatabaseLogger.CreateUserLogAsync(userId, $"User requested {parameters}");
                    }
                    else
                    {
                        DatabaseLogger.CreateUserLogAsync(null, $"Anon requested {parameters}");
                    }
                }
            }
            catch
            {
                // Swallow errors
            }

            base.OnActionExecuting(filterContext);
        }
    }
}