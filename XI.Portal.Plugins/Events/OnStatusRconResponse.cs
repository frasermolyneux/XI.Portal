using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnStatusRconResponse : ServerBaseEventArgs
    {
        public OnStatusRconResponse(Guid serverId, string serverName, GameType gameType, string responseData, bool monitorPlayers, bool monitorPlayerIPs) : base(serverId, serverName, gameType)
        {
            ResponseData = responseData;
            MonitorPlayers = monitorPlayers;
            MonitorPlayerIPs = monitorPlayerIPs;
        }

        public string ResponseData { get; }
        public bool MonitorPlayers { get; }
        public bool MonitorPlayerIPs { get; }
    }
}