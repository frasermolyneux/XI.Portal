﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Interfaces;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMaintenanceController : BaseController
    {
        private readonly IRconClientFactory rconClientFactory;

        public AdmMaintenanceController(
            IContextProvider contextProvider, IDatabaseLogger databaseLogger, IRconClientFactory rconClientFactory) : base(contextProvider, databaseLogger)
        {
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
        }

        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                ViewBag.SystemLogCount = await context.SystemLogs.CountAsync();
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

        public async Task<ActionResult> FtpCredentialCheck()
        {
            var results = new Dictionary<string, string>();

            using (var context = ContextProvider.GetContext())
            {
                var servers = await context.GameServers.ToListAsync();

                foreach (var gameServer in servers)
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

            return View(results);
        }

        public async Task<ActionResult> BanFileMonitorCheck()
        {
            var results = new Dictionary<string, string>();

            using (var context = ContextProvider.GetContext())
            {
                var banFileMonitors = await context.BanFileMonitors.Include(bfm => bfm.GameServer).ToListAsync();

                foreach (var banFileMonitor in banFileMonitors)
                    try
                    {
                        var request = (FtpWebRequest) WebRequest.Create($"ftp://{banFileMonitor.GameServer.FtpHostname}/{banFileMonitor.FilePath}");
                        request.Method = WebRequestMethods.Ftp.GetFileSize;
                        request.Credentials = new NetworkCredential(banFileMonitor.GameServer.FtpUsername, banFileMonitor.GameServer.FtpPassword);

                        var fileSize = ((FtpWebResponse) request.GetResponse()).ContentLength;

                        results.Add($"{banFileMonitor.GameServer.Title} - {banFileMonitor.FilePath}", $"Success, file size: {fileSize}");
                    }
                    catch (Exception ex)
                    {
                        results.Add($"{banFileMonitor.GameServer.Title} - {banFileMonitor.FilePath}", ex.Message);
                    }
            }

            return View(results);
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