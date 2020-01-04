using System;

namespace XI.Portal.Plugins.Events
{
    public class MonitorBaseEventArgs : EventArgs
    {
        public MonitorBaseEventArgs(Guid monitorId)
        {
            MonitorId = monitorId;
        }

        public Guid MonitorId { get; }
    }
}
