using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitor.Events
{
    public class OnChatMessageEventArgs : EventArgs
    {
        public string Guid { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Guid ServerId { get; set; }
        public ChatType ChatType { get; set; }
    }
}