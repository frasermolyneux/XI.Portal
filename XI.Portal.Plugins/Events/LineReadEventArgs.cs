using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class LineReadEventArgs : ServerBaseEventArgs
    {
        public LineReadEventArgs(Guid serverId, string serverName, GameType gameType, string lineData) : base(serverId, serverName, gameType)
        {
            LineData = lineData;
        }

        public string LineData { get; }
    }
}