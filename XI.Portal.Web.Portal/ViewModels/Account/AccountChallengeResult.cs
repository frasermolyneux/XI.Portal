using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace XI.Portal.Web.Portal.ViewModels.Account
{
    public class AccountChallengeResult : HttpUnauthorizedResult
    {
        public AccountChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
        {
        }

        public AccountChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; }
        public string RedirectUri { get; }
        public string UserId { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties {RedirectUri = RedirectUri};
            if (UserId != null) properties.Dictionary["XsrfId"] = UserId;

            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}