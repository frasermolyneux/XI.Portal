using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Web.Extensions;
using XI.Portal.Web.ViewModels;
using XI.Portal.Web.ViewModels.Servers;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class ServersController : BaseController
    {
        public ServersController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                var gameServers = await context.GameServers.Where(s => s.ShowOnPortalServerList).OrderBy(s => s.BannerServerListPosition).ToListAsync();
                var livePlayers = await context.LivePlayerLocations.ToListAsync();

                var model = new ServersIndexViewModel
                {
                    GameServers = gameServers,
                    LivePlayerLocations = livePlayers
                };

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ServerInfo(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var gameServer = await context.GameServers
                    .SingleOrDefaultAsync(server => server.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                var players = await context.LivePlayers
                    .Where(player => player.GameServer.ServerId == gameServer.ServerId)
                    .OrderByDescending(player => player.Score).ToListAsync();

                var currentMap = await context.Maps.Include(m => m.MapFiles).SingleOrDefaultAsync(m =>
                    m.MapName == gameServer.LiveMap && m.GameType == gameServer.GameType);

                var mapRotation = await context.MapRotations
                    .Include(m => m.Map)
                    .Include(m => m.Map.MapFiles)
                    .Include(m => m.Map.MapVotes)
                    .Where(m => m.GameServer.ServerId == gameServer.ServerId).ToListAsync();

                var mapDetails = new List<MapRotationViewModel>();

                foreach (var mapEntry in mapRotation)
                {
                    double totalLikes = mapEntry.Map.MapVotes.Where(mv => mv.Like).Count();
                    double totalDislikes = mapEntry.Map.MapVotes.Where(mv => !mv.Like).Count();
                    int totalVotes = mapEntry.Map.MapVotes.Count();
                    double likePercentage = 0;
                    double dislikePercentage = 0;

                    if (totalVotes > 0)
                    {
                        likePercentage = (totalLikes / totalVotes) * 100;
                        dislikePercentage = (totalDislikes / totalVotes) * 100;
                    }

                    mapDetails.Add(new MapRotationViewModel
                    {
                        MapRotation = mapEntry,
                        LikePercentage = likePercentage, 
                        DislikePercentage = dislikePercentage,
                        TotalLike = totalLikes,
                        TotalDislike = totalDislikes,
                        TotalVotes = totalVotes
                    });
                }

                var model = new ServerInfoViewModel
                {
                    GameServer = gameServer,
                    Players = players,
                    Map = currentMap,
                    MapRotation = mapDetails
                };

                return View(model);
            }
        }

        [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
        [HttpGet]
        public async Task<ActionResult> ChatLog(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var gameServer = await context.GameServers
                    .SingleOrDefaultAsync(server => server.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                return View(gameServer);
            }
        }

        [HttpGet]
        [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
        public async Task<ActionResult> GetChatLogAjax(string id, string sidx, string sord, int page, int rows,
            // ReSharper disable once InconsistentNaming
            bool _search, string searchField, string searchString, string searchOper)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return Json("Invalid Request", JsonRequestBehavior.AllowGet);

            using (var context = ContextProvider.GetContext())
            {
                var gameServer = await context.GameServers
                    .SingleOrDefaultAsync(server => server.ServerId == idAsGuid);

                if (gameServer == null)
                    return Json("Invalid Server", JsonRequestBehavior.AllowGet);

                var chatLogs = context.ChatLogs.Include(cl => cl.Player).Where(cl => cl.GameServer.ServerId == idAsGuid)
                    .OrderByDescending(cl => cl.Timestamp).AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                {
                    chatLogs = chatLogs.Where(cl => cl.Username.Contains(searchString) || cl.Message.Contains(searchString)).AsQueryable();
                }

                var totalRecords = chatLogs.Count();
                var skip = (page - 1) * rows;

                var chatLogList = await chatLogs.Skip(skip).Take(rows).ToListAsync();

                var chatLogsToReturn = chatLogList.Select(cl => new
                {
                    cl.Player.PlayerId,
                    Timestamp = cl.Timestamp.ToString(CultureInfo.InvariantCulture),
                    cl.Username,
                    Type = cl.ChatType.DisplayName(),
                    cl.Message,
                    cl.ChatLogId
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = chatLogsToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
        public async Task<ActionResult> ChatLogPermaLink(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var chatLog = await context.ChatLogs.Include(cl => cl.Player).Include(cl => cl.GameServer).SingleAsync(cl => cl.ChatLogId == idAsGuid);
                return View(chatLog);
            }
        }

        [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
        [HttpGet]
        public ActionResult GlobalChatLog()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
        public async Task<ActionResult> GetGlobalChatLogAjax(string sidx, string sord, int page, int rows,
            // ReSharper disable once InconsistentNaming
            bool _search, string searchField, string searchString, string searchOper)
        {
            using (var context = ContextProvider.GetContext())
            {
                var chatLogs = context.ChatLogs.Include(cl => cl.Player)
                    .OrderByDescending(cl => cl.Timestamp).AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                {
                    chatLogs = chatLogs.Where(cl => cl.Username.Contains(searchString) || cl.Message.Contains(searchString)).AsQueryable();
                }

                var totalRecords = chatLogs.Count();
                var skip = (page - 1) * rows;

                var chatLogList = await chatLogs.Skip(skip).Take(rows).ToListAsync();

                var chatLogsToReturn = chatLogList.Select(cl => new
                {
                    cl.Player.PlayerId,
                    Timestamp = cl.Timestamp.ToString(CultureInfo.InvariantCulture),
                    cl.Username,
                    Type = cl.ChatType.DisplayName(),
                    cl.Message,
                    cl.ChatLogId
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = chatLogsToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}