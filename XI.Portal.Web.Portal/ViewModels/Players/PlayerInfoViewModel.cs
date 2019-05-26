using System.Collections.Generic;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.GeoLocation.Models;

namespace XI.Portal.Web.Portal.ViewModels.Players
{
    public class PlayerInfoViewModel
    {
        public Player2 Player { get; set; }
        public LocationDto Location { get; set; }
        public List<PlayerAlias> Aliases { get; set; }
        public List<PlayerIpAddress> IpAddresses { get; set; }
        public List<AdminAction> AdminActions { get; set; }
    }
}