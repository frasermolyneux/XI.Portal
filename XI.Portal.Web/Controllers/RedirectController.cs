using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    public class RedirectController : BaseController
    {
        public RedirectController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        public ActionResult RedirectToUrl(string url)
        {
            var whitelistedDomains = new List<string>
            {
                "xtremeidiots.com",
                "xtremeidiots.net"
            };

            if (!whitelistedDomains.Any(url.Contains)) return RedirectToAction("Index", "Landing");

            return Redirect(url);
        }
    }
}