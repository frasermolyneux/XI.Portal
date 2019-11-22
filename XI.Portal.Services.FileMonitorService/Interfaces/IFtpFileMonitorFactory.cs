using System;
using System.Threading;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitorService.Interfaces
{
    public interface IFtpFileMonitorFactory
    {
        FtpFileMonitor.FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid serverId, string serverName, GameType gasmeType, CancellationTokenSource cancellationTokenSource);
    }
}