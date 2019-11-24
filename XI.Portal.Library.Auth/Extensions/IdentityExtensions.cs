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
            if (!identity.IsAuthenticated) return string.Empty;
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.Id);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsTitle(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.Title);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsFormattedName(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.FormattedName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupId(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupId);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupName(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPrimaryGroupIdFormattedName(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PrimaryGroupIdFormattedName);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPhotoUrl(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "Anon";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PhotoUrl);
            return claim != null ? claim.Value : string.Empty;
        }

        public static string XtremeIdiotsPhotoUrlIsDefault(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return "true";
            var claim = ((ClaimsIdentity) identity).FindFirst(XtremeIdiotsClaims.PhotoUrlIsDefault);
            return claim != null ? claim.Value : string.Empty;
        }

        public static bool IsInAdminOrModeratorRole(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return false;
            return XtremeIdiotsRolesHelper.IsGroupIdAdminOrModerator(Convert.ToInt32(identity.XtremeIdiotsPrimaryGroupId()));
        }

        public static bool IsInSeniorAdminRole(this IIdentity identity)
        {
            if (!identity.IsAuthenticated) return false;
            return XtremeIdiotsRolesHelper.IsGroupIdSeniorAdmin(Convert.ToInt32(identity.XtremeIdiotsPrimaryGroupId()));
        }
    }
}