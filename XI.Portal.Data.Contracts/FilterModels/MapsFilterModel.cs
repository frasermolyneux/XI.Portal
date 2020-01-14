using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Data.Contracts.FilterModels
{
    public class MapsFilterModel
    {
        public GameType GameType { get; set; }
        public OrderBy Order { get; set; } = OrderBy.MapNameAsc;
        public string FilterString { get; set; }
        public int SkipEntries { get; set; } = 0;
        public int TakeEntries { get; set; } = 0;

        public enum OrderBy
        {
            MapNameAsc,
            MapNameDesc,
            LikeDislikeAsc,
            LikeDislikeDesc,
            GameTypeAsc,
            GameTypeDesc
        }
    }
}
