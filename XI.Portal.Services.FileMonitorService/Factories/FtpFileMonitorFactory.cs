﻿using System;
using System.Threading;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Services.FileMonitorService.Interfaces;

namespace XI.Portal.Services.FileMonitorService.Factories
{
    public class FtpFileMonitorFactory : IFtpFileMonitorFactory
    {
        private readonly ILogger logger;

        public FtpFileMonitorFactory(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public FtpFileMonitor.FtpFileMonitor CreateInstance(string requestPath, string ftpUsername, string ftpPassword, Guid serverId, string serverName, GameType gameType, CancellationTokenSource cancellationTokenSource)
        {
            var ftpFileMonitor = new FtpFileMonitor.FtpFileMonitor(logger);
            ftpFileMonitor.Configure(requestPath, ftpUsername, ftpPassword, serverId, serverName, gameType, cancellationTokenSource);
            return ftpFileMonitor;
        }
    }
}