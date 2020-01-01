using System;
using System.Data.Entity;
using System.Linq;
using Serilog;
using XI.Portal.App.SyncService.BanFiles;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Ftp.Interfaces;

namespace XI.Portal.App.SyncService.Service
{
    public class BanSyncCoordinator : IBanSyncCoordinator
    {
        private readonly IBanFileImporter banFileImporter;
        private readonly IFtpHelper ftpHelper;
        private readonly IContextProvider contextProvider;
        private readonly ILocalBanFileManager localBanFileManager;
        private readonly ILogger logger;

        public BanSyncCoordinator(ILogger logger, IContextProvider contextProvider,
            ILocalBanFileManager localBanFileManager,
            IBanFileImporter banFileImporter,
            IFtpHelper ftpHelper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.banFileImporter = banFileImporter ?? throw new ArgumentNullException(nameof(banFileImporter));
            this.ftpHelper = ftpHelper ?? throw new ArgumentNullException(nameof(ftpHelper));
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

                var remoteBanFileSize = ftpHelper.GetFileSize(banFileMonitor.GameServer.Hostname, 
                    banFileMonitor.FilePath, 
                    banFileMonitor.GameServer.FtpUsername, 
                    banFileMonitor.GameServer.FtpPassword);

                var localBanFileSize = localBanFileManager.GetLocalBanFileSize(gameType);

                if (remoteBanFileSize == 0)
                {
                    ftpHelper.UpdateRemoteFile(banFileMonitor.GameServer.Hostname,
                        banFileMonitor.FilePath,
                        banFileMonitor.GameServer.FtpUsername,
                        banFileMonitor.GameServer.FtpPassword,
                        gameType.DataPath());
                }
                else if (remoteBanFileSize != localBanFileSize)
                {
                    logger.Information(
                        $"Remote filesize {remoteBanFileSize} does not match local filesize {localBanFileSize}");

                    var remoteBanFileData = ftpHelper.GetRemoteFileData(banFileMonitor.GameServer.Hostname,
                        banFileMonitor.FilePath,
                        banFileMonitor.GameServer.FtpUsername,
                        banFileMonitor.GameServer.FtpPassword);

                    banFileImporter.ImportData(gameType, remoteBanFileData, banFileMonitor.GameServer.ServerId);
                    localBanFileManager.GenerateBanFile(gameType);

                    ftpHelper.UpdateRemoteFile(banFileMonitor.GameServer.Hostname,
                        banFileMonitor.FilePath,
                        banFileMonitor.GameServer.FtpUsername,
                        banFileMonitor.GameServer.FtpPassword,
                        gameType.DataPath());
                }
                else
                {
                    logger.Information($"Remote ban file is up to date with size {remoteBanFileSize}");
                }
            }
        }
    }
}