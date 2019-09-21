using System;
using System.Threading;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Services.FileMonitor.Interfaces;

namespace XI.Portal.Services.FileMonitor.Factories
{
    public class FtpFileMonitorFactory : IFtpFileMonitorFactory
    {
        private readonly ILogger logger;

        public FtpFileMonitorFactory(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public FtpFileMonitor.FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid gameServerId, GameType gameServerGameType, CancellationTokenSource cancellationTokenSource)
        {
            var ftpFileMonitor = new FtpFileMonitor.FtpFileMonitor(logger);
            ftpFileMonitor.Configure(requestPath, ftpUsername, ftpPassword, gameServerId, gameServerGameType, cancellationTokenSource);
            return ftpFileMonitor;
        }
    }
}