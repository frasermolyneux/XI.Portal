using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Ftp.Interfaces;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Web.ViewModels.Monitor;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.Admins)]
    public class MonitorController : BaseController
    {
        private readonly IAdminActionsRepository adminActionsRepository;
        private readonly IFtpHelper ftpHelper;
        private readonly IRconClientFactory rconClientFactory;

        public MonitorController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IAdminActionsRepository adminActionsRepository,
            IFtpHelper ftpHelper,
            IRconClientFactory rconClientFactory) : base(contextProvider, databaseLogger)
        {
            this.adminActionsRepository = adminActionsRepository ?? throw new ArgumentNullException(nameof(adminActionsRepository));
            this.ftpHelper = ftpHelper ?? throw new ArgumentNullException(nameof(ftpHelper));
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
        }

        public async Task<ActionResult> BanFileMonitor()
        {
            var results = new List<BanFileMonitorStatusViewModel>();

            using (var context = ContextProvider.GetContext())
            {
                var banFileMonitors = await context.BanFileMonitors
                    .Include(bfm => bfm.GameServer)
                    .OrderBy(bfm => bfm.GameServer.BannerServerListPosition)
                    .ToListAsync();

                foreach (var banFileMonitor in banFileMonitors)
                    try
                    {
                        var fileSize = ftpHelper.GetFileSize(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);
                        var lastModified = ftpHelper.GetLastModified(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);

                        var lastBans = await adminActionsRepository.GetAdminActions(new AdminActionsFilterModel
                        {
                            Filter = AdminActionsFilterModel.FilterType.ActiveBans,
                            GameType = banFileMonitor.GameServer.GameType,
                            Order = AdminActionsFilterModel.OrderBy.CreatedDesc,
                            SkipEntries = 0,
                            TakeEntries = 1
                        });

                        var lastBan = lastBans.FirstOrDefault();
                        var lastGameBan = DateTime.MinValue;

                        var errorMessage = string.Empty;
                        var warningMessage = string.Empty;

                        if (lastBan != null)
                        {
                            lastGameBan = lastBan.Created;

                            if (lastGameBan >= lastModified)
                                errorMessage = "OUT OF SYNC - There are new portal bans that have not been applied.";

                            if (fileSize != banFileMonitor.RemoteFileSize)
                                errorMessage = "OUT OF SYNC - The remote file size does not match the last sync size.";
                        }

                        if (banFileMonitor.LastSync < DateTime.UtcNow.AddMinutes(-30))
                            warningMessage = "WARNING - It has been more than 30 minutes since the server had a sync check";

                        results.Add(new BanFileMonitorStatusViewModel
                        {
                            BanFileMonitor = banFileMonitor,
                            GameServer = banFileMonitor.GameServer,
                            FileSize = fileSize,
                            LastModified = lastModified,
                            LastGameBan = lastGameBan,
                            ErrorMessage = errorMessage,
                            WarningMessage = warningMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BanFileMonitorStatusViewModel
                        {
                            BanFileMonitor = banFileMonitor,
                            GameServer = banFileMonitor.GameServer,
                            ErrorMessage = ex.Message
                        });
                    }
            }

            return View(results);
        }

        public async Task<ActionResult> LogFileMonitor()
        {
            var results = new List<FileMonitorStatusViewModel>();

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitors = await context.FileMonitors
                    .Include(fm => fm.GameServer)
                    .OrderBy(fm => fm.GameServer.BannerServerListPosition)
                    .ToListAsync();

                foreach (var fileMonitor in fileMonitors)
                    try
                    {
                        var fileSize = ftpHelper.GetFileSize(fileMonitor.GameServer.Hostname, fileMonitor.FilePath, fileMonitor.GameServer.FtpUsername, fileMonitor.GameServer.FtpPassword);
                        var lastModified = ftpHelper.GetLastModified(fileMonitor.GameServer.Hostname, fileMonitor.FilePath, fileMonitor.GameServer.FtpUsername, fileMonitor.GameServer.FtpPassword);

                        var errorMessage = string.Empty;
                        var warningMessage = string.Empty;

                        if (lastModified < DateTime.Now.AddHours(-1))
                            errorMessage = "INVESTIGATE - The log file has not been modified in over 1 hour.";

                        if (fileMonitor.LastRead < DateTime.UtcNow.AddMinutes(-15))
                            warningMessage = "WARNING - The file has not been read in the past 15 minutes";

                        if (fileMonitor.LastRead < DateTime.UtcNow.AddMinutes(-30))
                            errorMessage = "ERROR - The file has not been read in the past 30 minutes";

                        results.Add(new FileMonitorStatusViewModel
                        {
                            FileMonitor = fileMonitor,
                            GameServer = fileMonitor.GameServer,
                            FileSize = fileSize,
                            LastModified = lastModified,
                            ErrorMessage = errorMessage,
                            WarningMessage = warningMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new FileMonitorStatusViewModel
                        {
                            FileMonitor = fileMonitor,
                            GameServer = fileMonitor.GameServer,
                            ErrorMessage = ex.Message
                        });
                    }
            }

            return View(results);
        }

        public async Task<ActionResult> RconMonitor()
        {
            var results = new List<RconMonitorStatusViewModel>();

            using (var context = ContextProvider.GetContext())
            {
                var rconMonitors = await context.RconMonitors
                    .Include(rm => rm.GameServer)
                    .OrderBy(rm => rm.GameServer.BannerServerListPosition)
                    .ToListAsync();

                foreach (var rconMonitor in rconMonitors)
                {
                    try
                    {
                        var rconClient = rconClientFactory.CreateInstance(
                            rconMonitor.GameServer.GameType,
                            rconMonitor.GameServer.Title,
                            rconMonitor.GameServer.Hostname,
                            rconMonitor.GameServer.QueryPort,
                            rconMonitor.GameServer.RconPassword,
                            new List<TimeSpan>
                            {
                                TimeSpan.FromSeconds(1)
                            }
                        );

                        var commandResult = rconClient.PlayerStatus();

                        var errorMessage = string.Empty;

                        if (rconMonitor.LastUpdated < DateTime.UtcNow.AddMinutes(-15))
                            errorMessage = "ERROR - The rcon status has not been updated in the past 15 minutes";

                        if (string.IsNullOrWhiteSpace(commandResult))
                            errorMessage = "ERROR - The rcon command result is empty";

                        if (commandResult.Contains("Invalid password"))
                            errorMessage = "ERROR - Invalid rcon password";

                        results.Add(new RconMonitorStatusViewModel
                        {
                            RconMonitor = rconMonitor,
                            GameServer = rconMonitor.GameServer,
                            RconStatusResult = commandResult,
                            ErrorMessage = errorMessage
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new RconMonitorStatusViewModel
                        {
                            RconMonitor = rconMonitor,
                            GameServer = rconMonitor.GameServer,
                            ErrorMessage = ex.Message
                        });
                    }
                }
            }

            return View(results);
        }
    }
}