using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class LineReadEventArgs : ServerBaseEventArgs
    {
        public LineReadEventArgs(Guid serverId, GameType gameType, string lineData) : base(serverId, gameType)
        {
            LineData = lineData;
        }

        public string LineData { get; }
    }
}