using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnStatusRconResponse : ServerBaseEventArgs
    {
        public OnStatusRconResponse(Guid serverId, GameType gameType, string responseData, bool monitorPlayerIPs) : base(serverId, gameType)
        {
            ResponseData = responseData;
            MonitorPlayerIPs = monitorPlayerIPs;
        }

        public string ResponseData { get; }
        public bool MonitorPlayerIPs { get; }
    }
}