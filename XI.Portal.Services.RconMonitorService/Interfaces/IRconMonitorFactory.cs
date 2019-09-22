using System;
using System.Threading;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.RconMonitorService.Interfaces
{
    internal interface IRconMonitorFactory
    {
        IRconMonitor CreateInstance(GameType gameType, Guid serverId, string serverName, string hostname, int port, string rconPassword, bool monitorMapRotation, bool monitorPlayers, bool monitorPlayerIPs, CancellationTokenSource cancellationTokenSource);
    }
}