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
using XI.Portal.Web.ViewModels.Monitor;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.Admins)]
    public class MonitorController : BaseController
    {
        private readonly IAdminActionsRepository adminActionsRepository;
        private readonly IFtpHelper ftpHelper;

        public MonitorController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IAdminActionsRepository adminActionsRepository,
            IFtpHelper ftpHelper) : base(contextProvider, databaseLogger)
        {
            this.adminActionsRepository = adminActionsRepository ?? throw new ArgumentNullException(nameof(adminActionsRepository));
            this.ftpHelper = ftpHelper ?? throw new ArgumentNullException(nameof(ftpHelper));
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

                        var outOfSync = false;
                        var lastGameBan = DateTime.MinValue;

                        if (lastBan != null)
                        {
                            lastGameBan = lastBan.Created;

                            if (lastGameBan >= lastModified)
                                outOfSync = true;

                            if (fileSize != banFileMonitor.RemoteFileSize)
                                outOfSync = true;
                        }

                        results.Add(new BanFileMonitorStatusViewModel
                        {
                            BanFileMonitor = banFileMonitor,
                            GameServer = banFileMonitor.GameServer,
                            FileSize = fileSize,
                            LastModified = lastModified,
                            OutOfSync = outOfSync,
                            LastGameBan = lastGameBan
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new BanFileMonitorStatusViewModel
                        {
                            BanFileMonitor = banFileMonitor,
                            GameServer = banFileMonitor.GameServer,
                            HasError = true,
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

                        var somethingMayBeWrong = lastModified < DateTime.Now.AddHours(-1);

                        results.Add(new FileMonitorStatusViewModel
                        {
                            FileMonitor = fileMonitor,
                            GameServer = fileMonitor.GameServer,
                            FileSize = fileSize,
                            LastModified = lastModified,
                            SomethingMayBeWrong = somethingMayBeWrong
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new FileMonitorStatusViewModel
                        {
                            FileMonitor = fileMonitor,
                            GameServer = fileMonitor.GameServer,
                            HasError = true,
                            ErrorMessage = ex.Message
                        });
                    }
            }

            return View(results);
        }
    }
}