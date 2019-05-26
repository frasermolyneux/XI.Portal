﻿using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.Portal.ViewModels.Servers
{
    public class ServerInfoViewModel
    {
        public GameServer GameServer { get; set; }
        public List<LivePlayer> Players { get; set; }
        public Map Map { get; set; }
        public List<MapRotation> MapRotation { get; set; }
    }
}