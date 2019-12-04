using System;
using System.Threading;
using Serilog;
using XI.Portal.App.FileMonitorService.Interfaces;
using XI.Portal.Data.CommonTypes;
using XI.Portal.App.FileMonitorService.Monitors;

namespace XI.Portal.App.FileMonitorService.Factories
{
    public class FtpFileMonitorFactory : IFtpFileMonitorFactory
    {
        private readonly ILogger logger;

        public FtpFileMonitorFactory(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid serverId, string serverName, GameType gameType, CancellationTokenSource cancellationTokenSource)
        {
            var ftpFileMonitor = new FtpFileMonitor(logger);
            ftpFileMonitor.Configure(requestPath, ftpUsername, ftpPassword, serverId, serverName, gameType, cancellationTokenSource);
            return ftpFileMonitor;
        }
    }
}