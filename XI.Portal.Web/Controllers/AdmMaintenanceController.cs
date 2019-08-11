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
    }
}