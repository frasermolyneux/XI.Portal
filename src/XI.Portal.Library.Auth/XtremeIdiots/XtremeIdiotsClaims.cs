using System.Security.Claims;

namespace XI.Portal.Library.Auth.XtremeIdiots
{
    public static class XtremeIdiotsClaims
    {
        public static string Id = ClaimTypes.NameIdentifier;
        public static string Name = ClaimTypes.Name;
        public static string Title = "XtremeIdiots.Title";
        public static string FormattedName = "XtremeIdiots.FormattedName";
        public static string PrimaryGroupId = "XtremeIdiots.PrimaryGroupId";
        public static string PrimaryGroupName = "XtremeIdiots.PrimaryGroupName";
        public static string PrimaryGroupIdFormattedName = "XtremeIdiots.PrimaryGroupIdFormattedName";
        public static string Email = ClaimTypes.Email;
        public static string PhotoUrl = "XtremeIdiots.PhotoUrl";
        public static string PhotoUrlIsDefault = "XtremeIdiots.PhotoUrlIsDefault";
    }
}