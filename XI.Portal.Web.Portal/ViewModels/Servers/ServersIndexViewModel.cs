using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.Portal.ViewModels.Servers
{
    public class ServersIndexViewModel
    {
        public List<GameServer> GameServers { get; set; }
        public List<LivePlayerLocation> LivePlayerLocations { get; set; }
    }
}