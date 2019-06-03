using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Web.ViewModels.AdmFileMonitor;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmFileMonitorsController : BaseController
    {
        public AdmFileMonitorsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                var fileMonitors =
                    await context.FileMonitors.Include(server => server.GameServer).ToListAsync();
                return View(fileMonitors);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            using (var context = ContextProvider.GetContext())
            {
                var gameServers = await context.GameServers.ToListAsync();
                return View(new CreateFileMonitorViewModel
                {
                    GameServers = gameServers
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateFileMonitorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitor = new FileMonitor
                {
                    GameServer = context.GameServers.Single(server => server.ServerId == model.GameServerId),
                    FilePath = model.FilePath,
                    LastRead = DateTime.UtcNow.AddDays(-1)
                };

                context.FileMonitors.Add(fileMonitor);
                await context.SaveChangesAsync();
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has created a new file monitor: {fileMonitor.FileMonitorId}");

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitor =
                    await context.FileMonitors.SingleOrDefaultAsync(
                        s => s.FileMonitorId == idAsGuid);

                if (fileMonitor == null)
                    return RedirectToAction("Index");

                var model = new EditFileMonitorViewModel
                {
                    GameServers = await context.GameServers.ToListAsync(),
                    GameServerId = fileMonitor.GameServer.ServerId,
                    FileMonitorId = fileMonitor.FileMonitorId,
                    FilePath = fileMonitor.FilePath
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditFileMonitorViewModel model)
        {
            using (var context = ContextProvider.GetContext())
            {
                if (!ModelState.IsValid)
                {
                    model.GameServers = await context.GameServers.ToListAsync();
                    return View(model);
                }

                var fileMonitor = await context.FileMonitors.SingleOrDefaultAsync(s =>
                    s.FileMonitorId == model.FileMonitorId);

                if (fileMonitor == null)
                    return RedirectToAction("Index");

                fileMonitor.FilePath = model.FilePath;

                await context.SaveChangesAsync();
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has updated a file monitor: {fileMonitor.FileMonitorId}");

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitor =
                    await context.FileMonitors.SingleOrDefaultAsync(
                        s => s.FileMonitorId == idAsGuid);

                if (fileMonitor == null)
                    return RedirectToAction("Index");

                return View(fileMonitor);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var fileMonitor =
                    await context.FileMonitors.SingleOrDefaultAsync(
                        s => s.FileMonitorId == idAsGuid);

                if (fileMonitor == null)
                    return RedirectToAction("Index");

                context.FileMonitors.Remove(fileMonitor);
                await context.SaveChangesAsync();
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has deleted a file monitor: {id}");

                return RedirectToAction("Index");
            }
        }
    }
}