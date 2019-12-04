using System;
using System.Threading;
using XI.Portal.Data.CommonTypes;
using XI.Portal.App.FileMonitorService.Monitors;

namespace XI.Portal.App.FileMonitorService.Interfaces
{
    public interface IFtpFileMonitorFactory
    {
        FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid serverId, string serverName, GameType gasmeType, CancellationTokenSource cancellationTokenSource);
    }
}