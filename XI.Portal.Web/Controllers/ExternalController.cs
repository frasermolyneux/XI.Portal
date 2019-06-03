using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class ExternalController : BaseController
    {
        public ExternalController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [AllowAnonymous]
        public ActionResult LatestAdminActions()
        {
            using (var context = ContextProvider.GetContext())
            {
                var latestActions = context.AdminActions.Include(aa => aa.Player).Include(aa => aa.Admin).OrderByDescending(aa => aa.Created).Take(15).ToList();
                return View(latestActions);
            }
        }
    }
}