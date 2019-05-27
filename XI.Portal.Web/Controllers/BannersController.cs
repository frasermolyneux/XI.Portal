using System;
using System.Linq;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class BannersController : Controller
    {
        private readonly IContextProvider contextProvider;

        public BannersController(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        [AllowAnonymous]
        public ActionResult GameServersList()
        {
            using (var context = contextProvider.GetContext())
            {
                var gameServers = context.GameServers.Where(s => s.ShowOnBannerServerList).OrderBy(s => s.BannerServerListPosition)
                    .ToList();
                return View(gameServers);
            }
        }
    }
}