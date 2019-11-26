using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Models
{
    public class GetPlayerCountFilterModel
    {
        public GameType GameType { get; set; }
        public FilterType Filter { get; set; } = FilterType.None;
        public string FilterString { get; set; }

        public enum FilterType
        {
            None,
            UsernameAndGuid, 
            IpAddress
        }
    }
}
