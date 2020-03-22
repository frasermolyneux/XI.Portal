using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Library.Auth.Extensions
{
    public static class ApplicationUserExtensions
    {
        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this ApplicationUser user,
            UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.Title, user.XtremeIdiotsTitle));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.FormattedName, user.XtremeIdiotsFormattedName));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupId, user.XtremeIdiotsPrimaryGroupId));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupName, user.XtremeIdiotsPrimaryGroupName));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupIdFormattedName, user.XtremeIdiotsPrimaryGroupIdFormattedName));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.PhotoUrl, user.XtremeIdiotsPhotoUrl));
            userIdentity.AddClaim(new Claim(XtremeIdiotsClaims.PrimaryGroupName, user.XtremeIdiotsPhotoUrlIsDefault));

            return userIdentity;
        }
    }
}