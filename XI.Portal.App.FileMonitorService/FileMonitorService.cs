using System;
using System.Data.Entity;
using System.Threading;
using Serilog;
using XI.Portal.App.FileMonitorService.Interfaces;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;
using XI.Portal.Plugins.Events;

namespace XI.Portal.App.FileMonitorService
{
    internal class FileMonitorService
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly IFtpFileMonitorFactory ftpFileMonitorFactory;
        private readonly ILogger logger;
        private readonly IParserFactory parserFactory;

        private CancellationTokenSource cts;

        public FileMonitorService(ILogger logger, IContextProvider contextProvider, IDatabaseLogger databaseLogger, IFtpFileMonitorFactory ftpFileMonitorFactory, IParserFactory parserFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.ftpFileMonitorFactory = ftpFileMonitorFactory ?? throw new ArgumentNullException(nameof(ftpFileMonitorFactory));
            this.parserFactory = parserFactory ?? throw new ArgumentNullException(nameof(parserFactory));
        }

        public void Start()
        {
            logger.Information("Starting new File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Starting new File Monitor instance");

            cts = new CancellationTokenSource();

            using (var context = contextProvider.GetContext())
            {
                foreach (var fileMonitor in context.FileMonitors.Include(fm => fm.GameServer))
                {
                    logger.Information($"Creating FtpFileMonitor for ftp://{fileMonitor.FilePath}");

                    var ftpFileMonitor = ftpFileMonitorFactory.CreateInstance($"ftp://{fileMonitor.GameServer.FtpHostname}/{fileMonitor.FilePath}",
                        fileMonitor.GameServer.FtpUsername,
                        fileMonitor.GameServer.FtpPassword,
                        fileMonitor.GameServer.ServerId,
                        fileMonitor.GameServer.Title,
                        fileMonitor.GameServer.GameType,
                        cts);

                    ftpFileMonitor.LineRead += FtpFileMonitor_LineRead;
                }
            }
        }

        public void Stop()
        {
            logger.Information("Stopping current File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Stopping current File Monitor instance");

            cts?.Cancel();
        }

        public void Shutdown()
        {
            logger.Information("Shutting down current FileMonitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Shutting down current FileMonitor instance");

            cts?.Cancel();
        }

        private void FtpFileMonitor_LineRead(object sender, EventArgs e)
        {
            var eventData = (LineReadEventArgs)e;

            var parser = parserFactory.GetParserForGameType(eventData.GameType);
            parser.ParseLine(eventData.LineData, eventData.ServerId, eventData.ServerName, eventData.GameType);
        }
    }
}