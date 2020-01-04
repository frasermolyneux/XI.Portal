using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using XI.Portal.Data.Core.Context;
using XI.Portal.Plugins.Events;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Plugins.ImAlivePlugin
{
    public class ImAlivePlugin : IPlugin
    {
        private readonly ILogger logger;
        private readonly IContextProvider contextProvider;

        public Dictionary<Guid, DateTime> RconMonitors { get; set; } = new Dictionary<Guid, DateTime>();
        public Dictionary<Guid, DateTime> FileMonitors { get; set; } = new Dictionary<Guid, DateTime>();

        public ImAlivePlugin(ILogger logger, IContextProvider contextProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void RegisterEventHandlers(IPluginEvents events)
        {
            events.LineRead += EventsAll;
            events.Action += EventsAll;
            events.ChatMessage += EventsAll;
            events.Damage += EventsAll;
            events.Kill += EventsAll;
            events.MapRotationRconResponse += EventsAll;
            events.PlayerConnected += EventsAll;
            events.PlayerDisconnected += EventsAll;
            events.RoundStart += EventsAll;
            events.StatusRconResponse += EventsAll;
            events.Suicide += EventsAll;
            events.TeamKill += EventsAll;
        }

        private void EventsAll(object sender, System.EventArgs e)
        {
            var eventArgs = (MonitorBaseEventArgs)e;
            var monitorId = eventArgs.MonitorId;

            if (RconMonitors.ContainsKey(monitorId))
            {
                UpdateRconMonitorHeartbeat(monitorId);
            }
            else if (FileMonitors.ContainsKey(monitorId))
            {
                UpdateFileMonitorHeartbeat(monitorId);
            }
            else
            {
                using (var context = contextProvider.GetContext())
                {
                    var rconMonitor = context.RconMonitors.SingleOrDefault(rm => rm.RconMonitorId == monitorId);
                    var fileMonitor = context.FileMonitors.SingleOrDefault(fm => fm.FileMonitorId == monitorId);

                    if (rconMonitor != null)
                    {
                        RconMonitors.Add(monitorId, new DateTime());
                        UpdateRconMonitorHeartbeat(monitorId);
                    }
                    else if (fileMonitor != null)
                    {
                        FileMonitors.Add(monitorId, new DateTime());
                        UpdateFileMonitorHeartbeat(monitorId);
                    }
                    else
                    {
                        logger.Warning($"Unsupported monitor type or monitor has been deleted {monitorId}");
                    }
                }
            }
        }

        private void UpdateRconMonitorHeartbeat(Guid monitorId)
        {
            var entry = RconMonitors[monitorId];

            if (entry >= DateTime.UtcNow.AddMinutes(-5)) return;

            using (var context = contextProvider.GetContext())
            {
                var rconMonitor = context.RconMonitors.Single(rm => rm.RconMonitorId == monitorId);
                rconMonitor.LastUpdated = DateTime.UtcNow;
                context.SaveChanges();
            }
        }

        private void UpdateFileMonitorHeartbeat(Guid monitorId)
        {
            var entry = FileMonitors[monitorId];

            if (entry >= DateTime.UtcNow.AddMinutes(-5)) return;

            using (var context = contextProvider.GetContext())
            {
                var rconMonitor = context.FileMonitors.Single(fm => fm.FileMonitorId == monitorId);
                rconMonitor.LastRead = DateTime.UtcNow;
                context.SaveChanges();
            }
        }
    }
}
