using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitor.Events
{
    public class LineReadEventArgs : EventArgs
    {
        public Guid ServerId { get; set; }
        public GameType GameType { get; set; }
        public string LineData { get; set; }
    }
}