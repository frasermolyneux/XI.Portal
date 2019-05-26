using System.Web.Mvc;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class LandingController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}