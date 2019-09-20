using System;

namespace XI.Portal.Services.FileMonitor.Events
{
    public class OnPlayerConnectedEventArgs : EventArgs
    {
        public Guid ServerId { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
    }
}