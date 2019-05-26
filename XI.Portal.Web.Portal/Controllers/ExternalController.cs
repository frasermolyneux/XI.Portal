using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class ExternalController : Controller
    {
        private readonly IContextProvider contextProvider;

        public ExternalController(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        [AllowAnonymous]
        public ActionResult LatestAdminActions()
        {
            using (var context = contextProvider.GetContext())
            {
                var latestActions = context.AdminActions.Include(aa => aa.Player).Include(aa => aa.Admin).OrderByDescending(aa => aa.Created).Take(15).ToList();
                return View(latestActions);
            }
        }
    }
}