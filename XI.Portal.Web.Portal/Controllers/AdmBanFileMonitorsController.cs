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
using XI.Portal.Web.Portal.ViewModels.AdmBanFileMonitor;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmBanFileMonitorsController : Controller
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;

        public AdmBanFileMonitorsController(IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = contextProvider.GetContext())
            {
                var banFileMonitors =
                    await context.BanFileMonitors.Include(server => server.GameServer).ToListAsync();
                return View(banFileMonitors);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            using (var context = contextProvider.GetContext())
            {
                var gameServers = await context.GameServers.ToListAsync();
                return View(new CreateBanFileMonitorViewModel
                {
                    GameServers = gameServers
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateBanFileMonitorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var context = contextProvider.GetContext())
            {
                var banFileMonitor = new BanFileMonitor
                {
                    GameServer = context.GameServers.Single(server => server.ServerId == model.GameServerId),
                    FilePath = model.FilePath,
                    LastSync = DateTime.UtcNow.AddDays(-1)
                };

                context.BanFileMonitors.Add(banFileMonitor);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has created a new ban file monitor: {banFileMonitor.BanFileMonitorId}");

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var banFileMonitor =
                    await context.BanFileMonitors.SingleOrDefaultAsync(
                        s => s.BanFileMonitorId == idAsGuid);

                if (banFileMonitor == null)
                    return RedirectToAction("Index");

                var model = new EditBanFileMonitorViewModel
                {
                    GameServers = await context.GameServers.ToListAsync(),
                    GameServerId = banFileMonitor.GameServer.ServerId,
                    BanFileMonitorId = banFileMonitor.BanFileMonitorId,
                    FilePath = banFileMonitor.FilePath
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditBanFileMonitorViewModel model)
        {
            using (var context = contextProvider.GetContext())
            {
                if (!ModelState.IsValid)
                {
                    model.GameServers = await context.GameServers.ToListAsync();
                    return View(model);
                }

                var banFileMonitor = await context.BanFileMonitors.SingleOrDefaultAsync(s =>
                    s.BanFileMonitorId == model.BanFileMonitorId);

                if (banFileMonitor == null)
                    return RedirectToAction("Index");

                banFileMonitor.FilePath = model.FilePath;

                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has updated a ban file monitor: {banFileMonitor.BanFileMonitorId}");

                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var banFileMonitor =
                    await context.BanFileMonitors.SingleOrDefaultAsync(
                        s => s.BanFileMonitorId == idAsGuid);

                if (banFileMonitor == null)
                    return RedirectToAction("Index");

                return View(banFileMonitor);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = contextProvider.GetContext())
            {
                var banFileMonitor =
                    await context.BanFileMonitors.SingleOrDefaultAsync(
                        s => s.BanFileMonitorId == idAsGuid);

                if (banFileMonitor == null)
                    return RedirectToAction("Index");

                context.BanFileMonitors.Remove(banFileMonitor);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has deleted a ban file monitor: {id}");

                return RedirectToAction("Index");
            }
        }
    }
}