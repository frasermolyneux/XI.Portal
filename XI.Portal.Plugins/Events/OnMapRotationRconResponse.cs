using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnMapRotationRconResponse : ServerBaseEventArgs
    {
        public OnMapRotationRconResponse(Guid serverId, string serverName, GameType gameType, string responseData) : base(serverId, serverName, gameType)
        {
            ResponseData = responseData;
        }

        public string ResponseData { get; }
    }
}