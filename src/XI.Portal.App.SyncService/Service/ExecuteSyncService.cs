using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using Serilog;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.App.SyncService.Service
{
    internal class ExecuteSyncService
    {
        private readonly IBanSyncCoordinator banSyncCoordinator;
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly ILogger logger;

        private CancellationTokenSource cts;
        private Thread workingThread;

        public ExecuteSyncService(
            ILogger logger,
            IContextProvider contextProvider,
            IBanSyncCoordinator banSyncCoordinator,
            IDatabaseLogger databaseLogger)
        {
            this.banSyncCoordinator = banSyncCoordinator ?? throw new ArgumentNullException(nameof(banSyncCoordinator));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void Start()
        {
            logger.Information("Starting new Ban File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Starting new Ban File Monitor instance");

            cts = new CancellationTokenSource();
            workingThread = new Thread(() => StartWorking(cts.Token));
            workingThread.Start();
        }

        public void Stop()
        {
            logger.Information("Stopping current Ban File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Stopping current Ban File Monitor instance");

            cts?.Cancel();

            workingThread?.Abort();
        }

        public void Shutdown()
        {
            logger.Information("Shutting down current Ban File Monitor instance");
            databaseLogger.CreateSystemLogAsync("Info", "Shutting down current Ban File Monitor instance");

            cts?.Cancel();

            workingThread?.Abort();
        }

        private void StartWorking(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var context = contextProvider.GetContext())
                    {
                        var banFileMonitor = context.BanFileMonitors.Include(bfm => bfm.GameServer).OrderBy(bs => bs.LastSync).First();

                        logger.Information($"Starting sync for {banFileMonitor.GameServer.Title} at {DateTime.UtcNow}");

                        try
                        {
                            banSyncCoordinator.ExecuteBanSync(banFileMonitor.BanFileMonitorId);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Failed to process BanSync");

                            banFileMonitor.LastError = ex.Message;
                        }

                        logger.Information($"Completed sync for {banFileMonitor.GameServer.Title} at {DateTime.UtcNow}");

                        banFileMonitor.LastSync = DateTime.UtcNow;
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Exception around main loop - possible network issue");
                }

                Thread.Sleep(TimeSpan.FromSeconds(60));
            }
        }
    }
}