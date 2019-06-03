using System;
using System.Security.Claims;
using System.Security.Principal;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Library.Auth.Extensions
{
    public static class IdentityExtensions
    {
        public static string XtremeIdiotsId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.Id);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsTitle(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.Title);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsFormattedName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.FormattedName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupId);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupIdFormattedName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupIdFormattedName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPhotoUrl(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PhotoUrl);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPhotoUrlIsDefault(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PhotoUrlIsDefault);
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