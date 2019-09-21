using System;

namespace XI.Portal.Plugins.Events
{
    public class OnPlayerConnectedEventArgs : EventArgs
    {
        public Guid ServerId { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
    }
}