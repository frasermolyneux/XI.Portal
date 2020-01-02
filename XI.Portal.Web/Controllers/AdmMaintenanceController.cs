using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Library.Rcon.Interfaces;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMaintenanceController : BaseController
    {
        private readonly IRconClientFactory rconClientFactory;

        public AdmMaintenanceController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IRconClientFactory rconClientFactory) : base(contextProvider, databaseLogger)
        {
            this.rconClientFactory = rconClientFactory ?? throw new ArgumentNullException(nameof(rconClientFactory));
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
                        var request = (FtpWebRequest)WebRequest.Create($"ftp://{gameServer.FtpHostname}/");
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




    }
}