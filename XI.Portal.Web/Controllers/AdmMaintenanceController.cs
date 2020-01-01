using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Interfaces;
using XI.Portal.Web.ViewModels.AdmMaintenance;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMaintenanceController : BaseController
    {
        private readonly IRconClientFactory rconClientFactory;
        private readonly IAdminActionsRepository adminActionsRepository;

        public AdmMaintenanceController(
            IContextProvider contextProvider, 
            IDatabaseLogger databaseLogger, 
            IRconClientFactory rconClientFactory,
            IAdminActionsRepository adminActionsRepository) : base(contextProvider, databaseLogger)
        {
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
            this.adminActionsRepository = adminActionsRepository ?? throw new ArgumentNullException(nameof(adminActionsRepository));
        }

        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                ViewBag.SystemLogCount = await context.SystemLogs.CountAsync();
                ViewBag.UserLogCount = await context.UserLogs.CountAsync();
            }

            return View();
        }

        public async Task<ActionResult> PurgeSystemLogs()
        {
            using (var context = ContextProvider.GetContext())
            {
                await context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE SystemLogs");
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> PurgeUserLogs()
        {
            using (var context = ContextProvider.GetContext())
            {
                await context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE UserLogs");
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> FtpCredentialCheck()
        {
            var results = new Dictionary<string, string>();

            using (var context = ContextProvider.GetContext())
            {
                var servers = await context.GameServers.ToListAsync();

                foreach (var gameServer in servers)
                {
                    if (string.IsNullOrWhiteSpace(gameServer.FtpHostname))
                    {
                        results.Add(gameServer.Title, "FtpHostname is empty");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(gameServer.FtpUsername))
                    {
                        results.Add(gameServer.Title, "FtpUsername is empty");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(gameServer.FtpPassword))
                    {
                        results.Add(gameServer.Title, "FtpPassword is empty");
                        continue;
                    }

                    try
                    {
                        var request = (FtpWebRequest) WebRequest.Create($"ftp://{gameServer.FtpHostname}/");
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        request.Credentials = new NetworkCredential(gameServer.FtpUsername, gameServer.FtpPassword);
                        request.GetResponse();

                        results.Add(gameServer.Title, "Success");
                    }
                    catch (Exception ex)
                    {
                        results.Add(gameServer.Title, ex.Message);
                    }
                }
            }

            return View(results);
        }

        public async Task<ActionResult> BanFileMonitorCheck()
        {
            var results = new List<BanFileMonitorStatusViewModel>();

            using (var context = ContextProvider.GetContext())
            {
                var banFileMonitors = await context.BanFileMonitors.Include(bfm => bfm.GameServer).ToListAsync();

                foreach (var banFileMonitor in banFileMonitors)
                    try
                    {
                        var fileSize = GetFileSize(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);
                        var lastModified = GetLastModified(banFileMonitor.GameServer.FtpHostname, banFileMonitor.FilePath, banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);

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

        private long GetFileSize(string hostname, string filePath, string username, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create($"ftp://{hostname}/{filePath}");
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Credentials = new NetworkCredential(username, password);

             return ((FtpWebResponse)request.GetResponse()).ContentLength;
        }

        private DateTime GetLastModified(string hostname, string filePath, string username, string password)
        {
            var request = (FtpWebRequest)WebRequest.Create($"ftp://{hostname}/{filePath}");
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
            request.Credentials = new NetworkCredential(username, password);

            return ((FtpWebResponse)request.GetResponse()).LastModified;
        }

        public async Task<ActionResult> RconMonitorCheck()
        {
            var results = new Dictionary<string, string>();

            using (var context = ContextProvider.GetContext())
            {
                var rconMonitors = await context.RconMonitors.Include(bfm => bfm.GameServer).ToListAsync();

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

                        results.Add(rconMonitor.GameServer.Title, commandResult);
                    }
                    catch (Exception ex)
                    {
                        results.Add(rconMonitor.GameServer.Title, ex.Message);
                    }
                }
            }

            return View(results);
        }

        public async Task<ActionResult> FileMonitorCheck()
        {
            var results = new Dictionary<string, string>();

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitors = await context.FileMonitors.Include(bfm => bfm.GameServer).ToListAsync();

                foreach (var fileMonitor in fileMonitors)
                    try
                    {
                        var request = (FtpWebRequest) WebRequest.Create($"ftp://{fileMonitor.GameServer.FtpHostname}/{fileMonitor.FilePath}");
                        request.Method = WebRequestMethods.Ftp.GetFileSize;
                        request.Credentials = new NetworkCredential(fileMonitor.GameServer.FtpUsername, fileMonitor.GameServer.FtpPassword);

                        var fileSize = ((FtpWebResponse) request.GetResponse()).ContentLength;

                        results.Add($"{fileMonitor.GameServer.Title} - {fileMonitor.FilePath}", $"Success, file size: {fileSize}");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"{fileMonitor.GameServer.Title} - {fileMonitor.FilePath}", ex.Message);
                    }
            }

            return View(results);
        }
    }
}