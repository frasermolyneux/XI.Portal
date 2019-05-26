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
using XI.Portal.Web.Portal.ViewModels.AdmServers;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmServersController : Controller
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;

        public AdmServersController(IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = contextProvider.GetContext())
            {
                var gameServers = await context.GameServers.OrderBy(s => s.BannerServerListPosition).ToListAsync();
                return View(gameServers);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CreateGameServerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateGameServerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var context = contextProvider.GetContext())
            {
                var gameServer = new GameServer
                {
                    Title = model.Title,
                    ShowOnBannerServerList = model.ShowOnBannerServerList,
                    BannerServerListPosition = model.ServerListPosition,
                    HtmlBanner = model.HtmlBanner,
                    GameType = model.GameType,
                    Hostname = model.Hostname,
                    QueryPort = model.QueryPort,
                    ShowOnPortalServerList = model.ShowOnPortalServerList,
                    FtpHostname = model.FtpHostname,
                    FtpUsername = model.FtpUsername,
                    FtpPassword = model.FtpPassword,
                    RconPassword = model.RconPassword,
                    ShowChatLog = model.ShowChatLog,
                    LiveLastUpdated = DateTime.UtcNow
                };

                context.GameServers.Add(gameServer);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has created a new server: {gameServer.ServerId}");

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
                var gameServer = await context.GameServers.SingleOrDefaultAsync(s => s.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                var model = new EditGameServerViewModel
                {
                    ServerId = gameServer.ServerId,
                    Title = gameServer.Title,
                    ShowOnBannerServerList = gameServer.ShowOnBannerServerList,
                    ServerListPosition = gameServer.BannerServerListPosition,
                    HtmlBanner = gameServer.HtmlBanner,
                    GameType = gameServer.GameType,
                    Hostname = gameServer.Hostname,
                    QueryPort = gameServer.QueryPort,
                    ShowOnPortalServerList = gameServer.ShowOnPortalServerList,
                    FtpHostname = gameServer.FtpHostname,
                    FtpUsername = gameServer.FtpUsername,
                    FtpPassword = gameServer.FtpPassword,
                    RconPassword = gameServer.RconPassword,
                    ShowChatLog = gameServer.ShowChatLog
                };

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditGameServerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var context = contextProvider.GetContext())
            {
                var gameServer = await context.GameServers.SingleOrDefaultAsync(s => s.ServerId == model.ServerId);

                if (gameServer == null)
                    return RedirectToAction("Index");

                gameServer.Title = model.Title;
                gameServer.ShowOnBannerServerList = model.ShowOnBannerServerList;
                gameServer.BannerServerListPosition = model.ServerListPosition;
                gameServer.HtmlBanner = model.HtmlBanner;
                gameServer.GameType = model.GameType;
                gameServer.Hostname = model.Hostname;
                gameServer.QueryPort = model.QueryPort;
                gameServer.ShowOnPortalServerList = model.ShowOnPortalServerList;
                gameServer.FtpHostname = model.FtpHostname;
                gameServer.FtpUsername = model.FtpUsername;
                gameServer.FtpPassword = model.FtpPassword;
                gameServer.RconPassword = model.RconPassword;
                gameServer.ShowChatLog = model.ShowChatLog;

                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has updated a server: {gameServer.ServerId}");

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
                var gameServer = await context.GameServers.SingleOrDefaultAsync(s => s.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                return View(gameServer);
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
                var gameServer = await context.GameServers.SingleOrDefaultAsync(s => s.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                var livePlayers = context.LivePlayers.Where(player => player.GameServer.ServerId == idAsGuid);

                context.LivePlayers.RemoveRange(livePlayers);
                context.GameServers.Remove(gameServer);
                await context.SaveChangesAsync();
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has deleted a server: {id}");

                return RedirectToAction("Index");
            }
        }
    }
}