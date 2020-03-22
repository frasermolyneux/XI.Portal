using Serilog;
using System;
using System.Data.Entity;
using System.Linq;
using XI.Portal.App.SyncService.BanFiles;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Ftp.Interfaces;

namespace XI.Portal.App.SyncService.Service
{
    public class BanSyncCoordinator : IBanSyncCoordinator
    {
        private readonly IBanFileImporter banFileImporter;
        private readonly IFtpHelper ftpHelper;
        private readonly IAdminActionsRepository adminActionsRepository;
        private readonly IContextProvider contextProvider;
        private readonly ILocalBanFileManager localBanFileManager;
        private readonly ILogger logger;

        public BanSyncCoordinator(ILogger logger, IContextProvider contextProvider,
            ILocalBanFileManager localBanFileManager,
            IBanFileImporter banFileImporter,
            IFtpHelper ftpHelper, 
            IAdminActionsRepository adminActionsRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.banFileImporter = banFileImporter ?? throw new ArgumentNullException(nameof(banFileImporter));
            this.ftpHelper = ftpHelper ?? throw new ArgumentNullException(nameof(ftpHelper));
            this.adminActionsRepository = adminActionsRepository ?? throw new ArgumentNullException(nameof(adminActionsRepository));
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
                var localBanFileSize = localBanFileManager.GetLocalBanFileSize(gameType);

                var fileSize = ftpHelper.GetFileSize(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);
                var lastModified = ftpHelper.GetLastModified(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);

                var lastBans = adminActionsRepository.GetAdminActions(new AdminActionsFilterModel
                {
                    Filter = AdminActionsFilterModel.FilterType.ActiveBans,
                    GameType = gameType,
                    Order = AdminActionsFilterModel.OrderBy.CreatedDesc,
                    SkipEntries = 0,
                    TakeEntries = 1
                }).Result;

                var lastBan = lastBans.FirstOrDefault();

                if (lastBan?.Created > lastModified || localBanFileSize != fileSize)
                {
                    logger.Information($"Uploading new file to {banFileMonitor.FilePath} for server {banFileMonitor.GameServer.Title}");

                    ftpHelper.UpdateRemoteFile(banFileMonitor.GameServer.Hostname,
                        banFileMonitor.FilePath,
                        banFileMonitor.GameServer.FtpUsername,
                        banFileMonitor.GameServer.FtpPassword,
                        gameType.DataPath());

                    banFileMonitor.RemoteFileSize = localBanFileSize;
                    banFileMonitor.LastSync = DateTime.UtcNow;

                    context.SaveChanges();
                }
                else
                {
                    logger.Information($"Remote ban file is up to date");
                }
            }
        }
    }
}