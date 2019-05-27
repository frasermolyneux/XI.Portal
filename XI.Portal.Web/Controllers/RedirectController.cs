using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace XI.Portal.Web.Controllers
{
    public class RedirectController : Controller
    {
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