using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class LandingController : BaseController
    {
        public LandingController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    }
}