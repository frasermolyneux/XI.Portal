using System;
using System.Threading;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Services.RconMonitorService.Interfaces
{
    internal interface IRconMonitor : IPluginEvents
    {
        void Configure(Guid serverId, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource);
        void GetMapRotation();
    }
}