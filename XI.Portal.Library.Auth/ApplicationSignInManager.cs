using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Library.Auth
{
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
    }
}