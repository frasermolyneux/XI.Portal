using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Forums;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.Admins)]
    public class ForumsController : Controller
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly IManageTopics manageTopics;

        public ForumsController(IContextProvider contextProvider, IDatabaseLogger databaseLogger, IManageTopics manageTopics)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.manageTopics = manageTopics ?? throw new ArgumentNullException(nameof(manageTopics));
        }

        [HttpGet]
        public async Task<ActionResult> Create(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index", "Players");

            using (var context = contextProvider.GetContext())
            {
                var adminAction = await context.AdminActions.Include(aa => aa.Player).Include(aa => aa.Admin).Where(p => p.AdminActionId == idAsGuid).SingleAsync();

                await manageTopics.CreateTopicForAdminAction(idAsGuid);
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), $"User created an admin action topic id for {adminAction.AdminActionId}");

                return RedirectToAction("Details", "Players", new {id = adminAction.Player.PlayerId});
            }
        }
    }
}