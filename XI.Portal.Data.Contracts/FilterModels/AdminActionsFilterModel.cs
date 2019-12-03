using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Contracts.FilterModels
{
    public class AdminActionsFilterModel
    {
        public GameType GameType { get; set; }
        public FilterType Filter { get; set; }
        public OrderBy Order { get; set; } = OrderBy.Created;
        public int SkipEntries { get; set; } = 0;
        public int TakeEntries { get; set; } = 0;

        public enum FilterType
        {
            ActiveBans,
            UnclaimedBans
        }

        public enum OrderBy
        {
            Created
        }
    }
}
