using System;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class LineReadEventArgs : ServerBaseEventArgs
    {
        public LineReadEventArgs(Guid monitorId, Guid serverId, string serverName, GameType gameType, string lineData) : base(monitorId, serverId, serverName, gameType)
        {
            LineData = lineData;
        }

        public string LineData { get; }
    }
}