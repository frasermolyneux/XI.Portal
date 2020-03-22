using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [AllowAnonymous]
    public class CommunityController : BaseController
    {
        public CommunityController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

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