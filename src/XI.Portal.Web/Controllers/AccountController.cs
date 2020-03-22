using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth;
using XI.Portal.Library.Auth.Extensions;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Web.ViewModels.Account;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class AccountController : BaseController
    {
        private readonly ApplicationUserManager applicationUserManager;
        private readonly IAuthenticationManager authenticationManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            ApplicationUserManager applicationUserManager,
            IAuthenticationManager authenticationManager, 
            ApplicationRoleManager roleManager) : base(contextProvider, databaseLogger)
        {
            this.applicationUserManager = applicationUserManager ??
                                          throw new ArgumentNullException(nameof(applicationUserManager));
            this.authenticationManager = authenticationManager ??
                                         throw new ArgumentNullException(nameof(authenticationManager));
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), "User has logged off");
            }
            
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Landing");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            var challengeResult = new AccountChallengeResult(provider,
                Url.Action("ExLoginCallback", "Account", new {ReturnUrl = returnUrl}));
            return challengeResult;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExLoginCallback(string returnUrl)
        {
            try
            {
                if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Servers");

                var loginInfo = await authenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null) return RedirectToAction("Index", "Landing");

                var user = await applicationUserManager.FindByEmailAsync(loginInfo.Email);
                if (user != null)
                {
                    user.XtremeIdiotsId = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.Id).Value;

                    user.XtremeIdiotsTitle = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.Title).Value;

                    user.XtremeIdiotsFormattedName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.FormattedName).Value;

                    user.XtremeIdiotsPrimaryGroupId = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupId).Value;

                    user.XtremeIdiotsPrimaryGroupName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupName).Value;

                    user.XtremeIdiotsPrimaryGroupIdFormattedName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupIdFormattedName).Value;

                    user.XtremeIdiotsPhotoUrl = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PhotoUrl).Value;

                    user.XtremeIdiotsPhotoUrlIsDefault = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PhotoUrlIsDefault).Value;

                    if (user.DemoManagerAuthKey == null)
                        user.DemoManagerAuthKey = Guid.NewGuid().ToString();

                    if (user.SecurityStamp == null)
                        user.SecurityStamp = Guid.NewGuid().ToString();

                    user.Roles?.Clear();
                    applicationUserManager.Update(user);
                    AddRolesToUser(user);

                    await SignInAsync(user, true);
                    await DatabaseLogger.CreateUserLogAsync(user.Id, "User has logged in");

                    return RedirectToLocal(returnUrl);
                }

                user = new ApplicationUser
                {
                    UserName = loginInfo.DefaultUserName,
                    Email = loginInfo.Email,
                    XtremeIdiotsId = loginInfo.ExternalIdentity.Claims.Single(c => c.Type == XtremeIdiotsClaims.Id).Value,
                    XtremeIdiotsTitle =
                        loginInfo.ExternalIdentity.Claims.Single(c => c.Type == XtremeIdiotsClaims.Title).Value,
                    XtremeIdiotsFormattedName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.FormattedName).Value,
                    XtremeIdiotsPrimaryGroupId = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupId).Value,
                    XtremeIdiotsPrimaryGroupName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupName).Value,
                    XtremeIdiotsPrimaryGroupIdFormattedName = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PrimaryGroupIdFormattedName).Value,
                    XtremeIdiotsPhotoUrl = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PhotoUrl).Value,
                    XtremeIdiotsPhotoUrlIsDefault = loginInfo.ExternalIdentity.Claims
                        .Single(c => c.Type == XtremeIdiotsClaims.PhotoUrlIsDefault).Value,
                    DemoManagerAuthKey = Guid.NewGuid().ToString()
                };

                var result = await applicationUserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    user = await applicationUserManager.FindByEmailAsync(loginInfo.Email);

                    AddRolesToUser(user);

                    result = await applicationUserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, true);
                        await DatabaseLogger.CreateUserLogAsync(user.Id, "User has logged in for the first time");
                        return RedirectToLocal(returnUrl);
                    }
                }

                AddErrors(result);

                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

                return RedirectToAction("Index", "Servers");
            }
            catch (Exception ex)
            {
                await DatabaseLogger.CreateSystemLogAsync("Error", "ExLoginCallback Error", ex.Message);
                throw;
            }
        }

        private void AddRolesToUser(ApplicationUser user)
        {
            var primaryGroup = (XtremeIdiotsGroups) Convert.ToInt32(user.XtremeIdiotsPrimaryGroupId);

            if(!roleManager.RoleExists(primaryGroup.ToString()))
            {
                roleManager.Create(new IdentityRole() { Name = primaryGroup.ToString() });
            }

            applicationUserManager.AddToRole(user.Id, primaryGroup.ToString());
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Servers");
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await user.GenerateUserIdentityAsync(applicationUserManager);
            authenticationManager.SignIn(new AuthenticationProperties {IsPersistent = isPersistent}, identity);
        }
    }
}