using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Contracts.FilterModels
{
    public class PlayersFilterModel
    {
        public GameType GameType { get; set; }
        public FilterType Filter { get; set; } = FilterType.None;
        public OrderBy Order { get; set; } = OrderBy.LastSeen;
        public string FilterString { get; set; }
        public int SkipPlayers { get; set; } = 0;
        public int TakePlayers { get; set; } = 0;

        public enum FilterType
        {
            None,
            UsernameAndGuid,
            IpAddress
        }

        public enum OrderBy
        {
            LastSeen,
            Username
        }
    }
}
