using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMaintenanceController : BaseController
    {
        public AdmMaintenanceController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {

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
    }
}