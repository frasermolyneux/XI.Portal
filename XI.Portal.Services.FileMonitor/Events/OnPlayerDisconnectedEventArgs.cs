using System;

namespace XI.Portal.Services.FileMonitor.Events
{
    public class OnPlayerDisconnectedEventArgs : EventArgs
    {
        public Guid ServerId { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
    }
}