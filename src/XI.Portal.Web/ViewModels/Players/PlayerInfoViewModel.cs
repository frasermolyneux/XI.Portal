using System.Collections.Generic;
using FM.GeoLocation.Contract.Models;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Players
{
    public class PlayerInfoViewModel
    {
        public Player2 Player { get; set; }
        public GeoLocationDto Location { get; set; }
        public List<PlayerAlias> Aliases { get; set; }
        public List<PlayerIpAddress> IpAddresses { get; set; }
        public List<AdminAction> AdminActions { get; set; }
        public List<PlayerIpAddress> RelatedIpAddresses { get; set; }
    }
}