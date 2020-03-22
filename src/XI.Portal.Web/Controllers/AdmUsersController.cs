using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmUsersController : BaseController
    {
        public AdmUsersController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                var users = await context.Users.OrderBy(u => u.UserName).ToListAsync();
                return View(users);
            }
        }

        public async Task<ActionResult> SetToRegisteredUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var user = await context.Users.SingleAsync(u => u.Id == id);

                user.Roles.Clear();
                user.Logins.Clear();
                user.XtremeIdiotsPrimaryGroupId = "3";
                user.XtremeIdiotsPrimaryGroupName = "Registered User";
                user.XtremeIdiotsPrimaryGroupIdFormattedName = "Registered User";
                user.XtremeIdiotsTitle = "Registered User";
                user.XtremeIdiotsFormattedName = user.UserName;

                context.SaveChanges();

                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has set {user.UserName} to be a registered user");
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> LockUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var user = await context.Users.SingleAsync(u => u.Id == id);

                user.LockoutEnabled = true;
                user.LockoutEndDateUtc = DateTime.MaxValue;

                context.SaveChanges();

                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has locked {user.UserName} account");
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> UnlockUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var user = await context.Users.SingleAsync(u => u.Id == id);

                user.LockoutEnabled = false;
                user.LockoutEndDateUtc = DateTime.UtcNow.AddHours(-1);

                context.SaveChanges();

                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has unlocked {user.UserName} account");
            }

            return RedirectToAction("Index");
        }
    }
}