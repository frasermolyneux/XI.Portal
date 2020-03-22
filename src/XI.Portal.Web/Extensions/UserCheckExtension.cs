using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Unity;
using XI.Portal.Library.Auth;

namespace XI.Portal.Web.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class UserCheckExtension : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var applicationUserManager = UnityConfig.Container.Resolve<ApplicationUserManager>();
            var authenticationManager = UnityConfig.Container.Resolve<IAuthenticationManager>();

            var user = HttpContext.Current?.User?.Identity;

            if (user == null)
                return;

            var userMember = applicationUserManager.FindById(user.GetUserId());

            if (userMember == null)
                return;

            if (userMember.LockoutEndDateUtc > DateTime.UtcNow) filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {controller = "Error", action = "NoAuth"}));

            if (userMember.Roles.Count > 0) return;

            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}