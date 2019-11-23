using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels
{
    public class MapRotationViewModel
    {
        public MapRotation MapRotation { get; set; }
        public double LikePercentage { get; set; }
        public double DislikePercentage { get; set; }
        public double TotalLike { get; set; }
        public double TotalDislike { get; set; }
        public int TotalVotes { get; set; }
    }
}