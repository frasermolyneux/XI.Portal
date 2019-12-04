using System;
using System.Data.Entity;
using System.Linq;
using Serilog;
using XI.Portal.App.SyncService.BanFiles;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Data.Core.Context;

namespace XI.Portal.App.SyncService.Service
{
    public class BanSyncCoordinator : IBanSyncCoordinator
    {
        private readonly IBanFileImporter banFileImporter;
        private readonly IContextProvider contextProvider;
        private readonly ILocalBanFileManager localBanFileManager;
        private readonly ILogger logger;

        public BanSyncCoordinator(ILogger logger, IContextProvider contextProvider,
            ILocalBanFileManager localBanFileManager,
            IBanFileImporter banFileImporter)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.banFileImporter = banFileImporter ?? throw new ArgumentNullException(nameof(banFileImporter));
            this.localBanFileManager = localBanFileManager ??
                                       throw new ArgumentNullException(nameof(localBanFileManager));
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }

        public void ExecuteBanSync(Guid banFileMonitorId)
        {
            using (var context = contextProvider.GetContext())
            {
                var banFileMonitor = context.BanFileMonitors.Include(bfm => bfm.GameServer).Single(bs => bs.BanFileMonitorId == banFileMonitorId);
                var gameType = banFileMonitor.GameServer.GameType;

                localBanFileManager.GenerateBanFileIfRequired(gameType);

                var remoteBanFileSize = banFileMonitor.GetRemoteBanFileSize();
                var localBanFileSize = localBanFileManager.GetLocalBanFileSize(gameType);

                if (remoteBanFileSize == 0)
                {
                    banFileMonitor.UpdateFromLocal(gameType.DataPath());
                }
                else if (remoteBanFileSize != localBanFileSize)
                {
                    logger.Information(
                        $"Remote filesize {remoteBanFileSize} does not match local filesize {localBanFileSize}");

                    var remoteBanFileData = banFileMonitor.GetRemoteBanData();
                    banFileImporter.ImportData(gameType, remoteBanFileData, banFileMonitor.GameServer.ServerId);
                    localBanFileManager.GenerateBanFile(gameType);
                    banFileMonitor.UpdateFromLocal(gameType.DataPath());
                }
                else
                {
                    logger.Information($"Remote ban file is up to date with size {remoteBanFileSize}");
                }
            }
        }
    }
}