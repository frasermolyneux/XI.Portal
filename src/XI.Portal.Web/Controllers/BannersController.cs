using System.Linq;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class BannersController : BaseController
    {
        public BannersController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [AllowAnonymous]
        public ActionResult GameServersList()
        {
            using (var context = ContextProvider.GetContext())
            {
                var gameServers = context.GameServers.Where(s => s.ShowOnBannerServerList).OrderBy(s => s.BannerServerListPosition)
                    .ToList();
                return View(gameServers);
            }
        }
    }
}