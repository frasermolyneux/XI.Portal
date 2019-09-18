using System;
using System.Threading;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitor.Interfaces
{
    public interface IFtpFileMonitorFactory
    {
        FtpFileMonitor.FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid gameServerId, GameType gameServerGameType, CancellationTokenSource cancellationTokenSource);
    }
}