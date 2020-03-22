using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnMapRotationRconResponse : ServerBaseEventArgs
    {
        public OnMapRotationRconResponse(Guid monitorId, Guid serverId, string serverName, GameType gameType, string responseData) : base(monitorId, serverId, serverName, gameType)
        {
            ResponseData = responseData;
        }

        public string ResponseData { get; }
    }
}