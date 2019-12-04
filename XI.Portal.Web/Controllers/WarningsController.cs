using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.Extensions;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Library.Forums;
using XI.Portal.Library.Logging;
using XI.Portal.Web.ViewModels.Warnings;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.Admins)]
    public class WarningsController : BaseController
    {
        private readonly IManageTopics manageTopics;

        public WarningsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IManageTopics manageTopics) : base(contextProvider, databaseLogger)
        {
            this.manageTopics = manageTopics ?? throw new ArgumentNullException(nameof(manageTopics));
        }

        [HttpGet]
        public async Task<ActionResult> Create(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index", "Players");

            using (var context = ContextProvider.GetContext())
            {
                var player = await context.Players.Where(p => p.PlayerId == idAsGuid).SingleAsync();

                var model = new CreateWarningViewModel
                {
                    Player = player,
                    PlayerId = player.PlayerId,
                    Text = string.Empty
                };

                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateWarningViewModel model)
        {
            using (var context = ContextProvider.GetContext())
            {
                if (!ModelState.IsValid)
                {
                    model.Player = await context.Players.Where(p => p.PlayerId == model.PlayerId).SingleAsync();
                    return View(model);
                }

                var currentUserId = User.Identity.GetUserId();
                var adminAction = new AdminAction
                {
                    Player = await context.Players.Where(p => p.PlayerId == model.PlayerId).SingleAsync(),
                    Admin = await context.Users.Where(u => u.Id == currentUserId).SingleAsync(),
                    Type = AdminActionType.Warning,
                    Text = model.Text,
                    Created = DateTime.UtcNow,
                    Expires = null
                };

                context.AdminActions.Add(adminAction);
                await context.SaveChangesAsync();
                await manageTopics.CreateTopicForAdminAction(adminAction.AdminActionId);
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), $"Added a {adminAction.Type} to {adminAction.Player.Username} ({adminAction.Player.PlayerId})");

                return RedirectToAction("Details", "Players", new {id = model.PlayerId});
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index", "Players");

            using (var context = ContextProvider.GetContext())
            {
                var adminAction = await context.AdminActions.Include(a => a.Player).Where(a => a.AdminActionId == idAsGuid).SingleAsync();

                if (!User.Identity.IsInSeniorAdminRole() && User.Identity.GetUserId() != adminAction.Admin.Id)
                {
                    await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                        $"User attempted to edit an admin action they do not have permissions to {adminAction.AdminActionId}");
                    return RedirectToAction("Index", "Players");
                }

                var model = new EditWarningViewModel
                {
                    Player = adminAction.Player,
                    AdminActionId = adminAction.AdminActionId,
                    Text = adminAction.Text
                };

                return View(model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditWarningViewModel model)
        {
            using (var context = ContextProvider.GetContext())
            {
                var adminAction = await context.AdminActions.Include(a => a.Player).Where(a => a.AdminActionId == model.AdminActionId).SingleAsync();

                if (!User.Identity.IsInSeniorAdminRole() && User.Identity.GetUserId() != adminAction.Admin.Id)
                {
                    await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                        $"User attempted to edit an admin action they do not have permissions to {adminAction.AdminActionId}");
                    return RedirectToAction("Index", "Players");
                }

                if (!ModelState.IsValid)
                {
                    model.Player = adminAction.Player;
                    return View(model);
                }

                adminAction.Text = model.Text;

                await context.SaveChangesAsync();
                await manageTopics.UpdateTopicForAdminAction(model.AdminActionId);
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), $"Updated a {adminAction.Type} ({adminAction.AdminActionId}) on {adminAction.Player.Username} ({adminAction.Player.PlayerId})");

                return RedirectToAction("Details", "Players", new {id = adminAction.Player.PlayerId});
            }
        }
    }
}