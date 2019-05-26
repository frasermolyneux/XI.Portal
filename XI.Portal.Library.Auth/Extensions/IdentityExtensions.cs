using System;
using System.Security.Claims;
using System.Security.Principal;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Library.Auth.Extensions
{
    public static class IdentityExtensions
    {
        public static string XtremeIdiotsPrimaryGroupId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupId);
            return claim != null ? claim.Value : string.Empty;
        }

        public static bool IsInAdminOrModeratorRole(this IIdentity identity)
        {
            return XtremeIdiotsRolesHelper.IsGroupIdAdminOrModerator(Convert.ToInt32(identity.XtremeIdiotsPrimaryGroupId()));
        }

        public static bool IsInSeniorAdminRole(this IIdentity identity)
        {
            return XtremeIdiotsRolesHelper.IsGroupIdSeniorAdmin(Convert.ToInt32(identity.XtremeIdiotsPrimaryGroupId()));
        }
    }
}