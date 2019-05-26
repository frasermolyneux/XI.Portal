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
using XI.Portal.Web.Portal.ViewModels.AdmRconMonitor;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmRconMonitorsController : Controller
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;

        public AdmRconMonitorsController(IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = contextProvider.GetContext())
            {
                var rconMonitors =
                    await context.RconMonitors.Include(server => server.GameServer).ToListAsync();
                return View(rconMonitors);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            using (var context = contextProvider.GetContext())
            {
                var gameServers = await context.GameServers.ToListAsync();
                return View(new CreateRconMonitorViewModel
                {
                    GameServers = gameServers
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRconMonitorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var context = contextProvider.GetContext())
            {
                var rconMonitor = new RconMonitor
                {
                    GameServer = context.GameServers.Single(server => server.ServerId == model.GameServerId),
                    MonitorMapRotation = model.MonitorMapRotation,
                    MonitorPlayers = model.MonitorPlayers,
                    MonitorPlayerIPs = model.MonitorPlayerIPs,
                    LastUpdated = DateTime.UtcNow.AddDays(-1),
                    MapRotationLastUpdated = DateTime.UtcNow.AddDays(-1)
                };

                context.RconMonitors.Add(rconMonitor);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has created a new rcon monitor: {rconMonitor.RconMonitorId}");

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
                var rconMonitor =
                    await context.RconMonitors.SingleOrDefaultAsync(
                        s => s.RconMonitorId == idAsGuid);

                if (rconMonitor == null)
                    return RedirectToAction("Index");

                var model = new EditRconMonitorViewModel
                {
                    GameServers = await context.GameServers.ToListAsync(),
                    GameServerId = rconMonitor.GameServer.ServerId,
                    RconMonitorId = rconMonitor.RconMonitorId,
                    MonitorMapRotation = rconMonitor.MonitorMapRotation,
                    MonitorPlayers = rconMonitor.MonitorPlayers,
                    MonitorPlayerIPs = rconMonitor.MonitorPlayerIPs
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditRconMonitorViewModel model)
        {
            using (var context = contextProvider.GetContext())
            {
                if (!ModelState.IsValid)
                {
                    model.GameServers = await context.GameServers.ToListAsync();
                    return View(model);
                }

                var rconMonitor = await context.RconMonitors.SingleOrDefaultAsync(s =>
                    s.RconMonitorId == model.RconMonitorId);

                if (rconMonitor == null)
                    return RedirectToAction("Index");

                rconMonitor.MonitorMapRotation = model.MonitorMapRotation;
                rconMonitor.MonitorPlayers = model.MonitorPlayers;
                rconMonitor.MonitorPlayerIPs = model.MonitorPlayerIPs;

                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has updated a rcon monitor: {rconMonitor.RconMonitorId}");

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
                var rconMonitor =
                    await context.RconMonitors.SingleOrDefaultAsync(
                        s => s.RconMonitorId == idAsGuid);

                if (rconMonitor == null)
                    return RedirectToAction("Index");

                return View(rconMonitor);
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
                var rconMonitor =
                    await context.RconMonitors.SingleOrDefaultAsync(
                        s => s.RconMonitorId == idAsGuid);

                if (rconMonitor == null)
                    return RedirectToAction("Index");

                context.RconMonitors.Remove(rconMonitor);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has deleted a rcon monitor: {id}");

                return RedirectToAction("Index");
            }
        }
    }
}