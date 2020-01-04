using System;
using System.Threading;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.RconMonitorService.Interfaces
{
    internal interface IRconMonitorFactory
    {
        IRconMonitor CreateInstance(Guid monitorId, GameType gameType, Guid serverId, string serverName, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource);
    }
}