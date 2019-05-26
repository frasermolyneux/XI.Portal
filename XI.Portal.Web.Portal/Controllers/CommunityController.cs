using System.Web.Mvc;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class CommunityController : Controller
    {
        [HttpGet]
        public ActionResult Clubs()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Teamspeak()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Discord()
        {
            return View();
        }
    }
}