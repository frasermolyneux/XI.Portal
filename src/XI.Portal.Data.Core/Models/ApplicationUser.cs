using Microsoft.AspNet.Identity.EntityFramework;

namespace XI.Portal.Data.Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string XtremeIdiotsId { get; set; }
        public string XtremeIdiotsTitle { get; set; }
        public string XtremeIdiotsFormattedName { get; set; }
        public string XtremeIdiotsPrimaryGroupId { get; set; }
        public string XtremeIdiotsPrimaryGroupName { get; set; }
        public string XtremeIdiotsPrimaryGroupIdFormattedName { get; set; }
        public string XtremeIdiotsPhotoUrl { get; set; }
        public string XtremeIdiotsPhotoUrlIsDefault { get; set; }

        public string DemoManagerAuthKey { get; set; }
    }
}