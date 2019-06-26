using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.GeoLocation.Extensions;
using XI.Portal.Library.GeoLocation.Repository;
using XI.Portal.Library.Logging;
using XI.Portal.Web.ViewModels.Players;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.AdminAndModerators)]
    public class PlayersController : BaseController
    {
        private readonly IGeoLocationApiRepository geoLocationApiRepository;

        public PlayersController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IGeoLocationApiRepository geoLocationApiRepository) : base(contextProvider, databaseLogger)
        {
            this.geoLocationApiRepository = geoLocationApiRepository ?? throw new ArgumentNullException(nameof(geoLocationApiRepository));
        }

        [HttpGet]
        public async Task<ActionResult> Home()
        {
            using (var context = ContextProvider.GetContext())
            {
                var model = new PlayersIndexViewModel
                {
                    TotalTrackedPlayers = await context.Players.CountAsync(),
                    TotalOnlinePlayers = await context.LivePlayers.CountAsync(),
                    TotalBannedPlayers = await context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null
                                                                                || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow).CountAsync(),
                    TotalUnclaimedBans = await context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null && aa.Admin == null).CountAsync()
                };

                var games = await context.Players.Select(p => p.GameType).Distinct().ToListAsync();

                foreach (var gameType in games.OrderBy(gt => gt.ToString()))
                {
                    var gameModel = new GameIndexViewModel
                    {
                        GameType = gameType,
                        TrackedPlayers = await context.Players.Where(p => p.GameType == gameType).CountAsync(),
                        BannedPlayers = await context.AdminActions.Where(aa => aa.Player.GameType == gameType
                                                                               && (aa.Type == AdminActionType.Ban && aa.Expires == null
                                                                                   || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow)).CountAsync()
                    };

                    model.GameIndexViewModels.Add(gameModel);
                }

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Index(GameType id)
        {
            return View(id);
        }

        [HttpGet]
        public async Task<ActionResult> GetPlayersAjax(GameType id, string sidx, string sord, int page, int rows,
            // ReSharper disable once InconsistentNaming
            bool _search, string searchField, string searchString, string searchOper)
        {
            using (var context = ContextProvider.GetContext())
            {
                var players = context.Players.Where(p => p.GameType == id)
                    .OrderByDescending(cl => cl.LastSeen).AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                    switch (searchField)
                    {
                        case "Username":
                            players = searchOper == "eq"
                                ? players.Where(cl => cl.Username == searchString).AsQueryable()
                                : players.Where(cl => cl.Username.Contains(searchString)).AsQueryable();
                            break;
                        case "Guid":
                            players = searchOper == "eq"
                                ? players.Where(cl => cl.Guid == searchString).AsQueryable()
                                : players.Where(cl => cl.Guid.Contains(searchString)).AsQueryable();
                            break;
                    }

                var totalRecords = players.Count();
                var skip = (page - 1) * rows;

                var playerList = await players.Skip(skip).Take(rows).ToListAsync();

                var playersToReturn = playerList.Select(p => new
                {
                    p.PlayerId,
                    p.Username,
                    p.Guid,
                    FirstSeen = p.FirstSeen.ToString(CultureInfo.InvariantCulture),
                    LastSeen = p.LastSeen.ToString(CultureInfo.InvariantCulture)
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = playersToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult IPSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetPlayersIPSearchAjax(string sidx, string sord, int page, int rows,
            // ReSharper disable once InconsistentNaming
            bool _search, string searchField, string searchString, string searchOper)
        {
            using (var context = ContextProvider.GetContext())
            {
                var ipAddresses = context.PlayerIpAddresses.OrderByDescending(ip => ip.LastUsed).AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                    switch (searchField)
                    {
                        case "Address":
                            ipAddresses = ipAddresses.Where(ip => ip.Address.Contains(searchString)).AsQueryable();
                            break;
                    }

                var totalRecords = ipAddresses.Count();
                var skip = (page - 1) * rows;

                var playerIpAddresses = await ipAddresses.Skip(skip).Take(rows).Include(p => p.Player).ToListAsync();

                var playersToReturn = playerIpAddresses.Select(ip => new
                {
                    GameType = ip.Player.GameType.ToString(),
                    ip.Player.PlayerId,
                    ip.Player.Username,
                    ip.Address
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = playersToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Banned(GameType id)
        {
            return View(id);
        }

        [HttpGet]
        public async Task<ActionResult> GetBannedPlayersAjax(GameType id, string sidx, string sord, int page, int rows,
            // ReSharper disable once InconsistentNaming
            bool _search, string searchField, string searchString, string searchOper)
        {
            using (var context = ContextProvider.GetContext())
            {
                var adminActions = context.AdminActions.Include(aa => aa.Player)
                    .Where(aa => aa.Player.GameType == id
                                 && (aa.Type == AdminActionType.Ban && aa.Expires == null
                                     || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow))
                    .OrderByDescending(aa => aa.Created)
                    .AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                    switch (searchField)
                    {
                        case "Username":
                            adminActions = searchOper == "eq"
                                ? adminActions.Where(aa => aa.Player.Username == searchString).AsQueryable()
                                : adminActions.Where(aa => aa.Player.Username.Contains(searchString)).AsQueryable();
                            break;
                        case "Guid":
                            adminActions = searchOper == "eq"
                                ? adminActions.Where(aa => aa.Player.Guid == searchString).AsQueryable()
                                : adminActions.Where(aa => aa.Player.Guid.Contains(searchString)).AsQueryable();
                            break;
                    }

                var totalRecords = adminActions.Count();
                var skip = (page - 1) * rows;

                var playerList = await adminActions.Skip(skip).Take(rows).ToListAsync();

                var playersToReturn = playerList.Select(aa => new
                {
                    aa.Player.PlayerId,
                    aa.Player.Username,
                    aa.Player.Guid,
                    Type = aa.Type.ToString(),
                    Expires = aa.Type == AdminActionType.Ban ? "Never" : aa.Expires.ToString()
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = playersToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out var idAsGuid))
                    return RedirectToAction("Home");

                using (var context = ContextProvider.GetContext())
                {
                    var player = await context.Players.SingleOrDefaultAsync(p => p.PlayerId == idAsGuid);

                    if (player == null)
                        return RedirectToAction("Home");

                    var model = new PlayerInfoViewModel
                    {
                        Player = player,
                        Aliases = await context.PlayerAliases.Where(pa => pa.Player.PlayerId == player.PlayerId)
                            .OrderByDescending(pa => pa.LastUsed).ToListAsync(),

                        IpAddresses = await context.PlayerIpAddresses.Where(pip => pip.Player.PlayerId == player.PlayerId)
                            .OrderByDescending(pip => pip.LastUsed).ToListAsync(),

                        AdminActions = await context.AdminActions.Include(aa => aa.Admin).Where(aa => aa.Player.PlayerId == player.PlayerId)
                            .OrderByDescending(aa => aa.Created).ToListAsync()
                    };

                    model.RelatedIpAddresses = new List<PlayerIpAddress>();
                    foreach (var playerIpAddress in model.IpAddresses)
                    {
                        var relatedPlayersFromIp = await context.PlayerIpAddresses.Include(ip => ip.Player).Where(ip => ip.Address == playerIpAddress.Address && ip.Player.PlayerId != idAsGuid).ToListAsync();
                        model.RelatedIpAddresses.AddRange(relatedPlayersFromIp);
                    }

                    var playerLocation = await geoLocationApiRepository.GetLocation(player.IpAddress);

                    if (!playerLocation.IsDefault())
                        model.Location = playerLocation;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                await DatabaseLogger.CreateSystemLogAsync("Error", "[Portal] Unhandled Error", ex);
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
        public async Task<ActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Home");

            using (var context = ContextProvider.GetContext())
            {
                var player = await context.Players.SingleOrDefaultAsync(p => p.PlayerId == idAsGuid);

                if (player == null)
                    return RedirectToAction("Home");

                return View(player);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Home");

            using (var context = ContextProvider.GetContext())
            {
                var player = await context.Players.SingleOrDefaultAsync(p => p.PlayerId == idAsGuid);

                if (player == null)
                    return RedirectToAction("Home");

                context.ChatLogs.RemoveRange(context.ChatLogs.Where(cl => cl.Player.PlayerId == player.PlayerId));
                context.PlayerAliases.RemoveRange(context.PlayerAliases.Where(pa => pa.Player.PlayerId == player.PlayerId));
                context.PlayerIpAddresses.RemoveRange(context.PlayerIpAddresses.Where(pip => pip.Player.PlayerId == player.PlayerId));
                context.AdminActions.RemoveRange(context.AdminActions.Where(aa => aa.Player.PlayerId == player.PlayerId));
                context.Players.Remove(player);

                await context.SaveChangesAsync();
                await DatabaseLogger.CreateUserLogAsync(User.Identity.GetUserId(), $"User has deleted a player: {id}");

                return RedirectToAction("Home");
            }
        }

        [HttpGet]
        public async Task<ActionResult> MyActions()
        {
            using (var context = ContextProvider.GetContext())
            {
                var currentUserId = User.Identity.GetUserId();
                var adminActions = await context.AdminActions
                    .Include(aa => aa.Player)
                    .Include(aa => aa.Admin)
                    .Where(aa => aa.Admin.Id == currentUserId)
                    .OrderByDescending(aa => aa.Created).ToListAsync();

                return View(adminActions);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Unclaimed()
        {
            using (var context = ContextProvider.GetContext())
            {
                var adminActions = await context.AdminActions
                    .Include(aa => aa.Player)
                    .Include(aa => aa.Admin)
                    .Where(aa => aa.Type == AdminActionType.Ban && aa.Admin == null)
                    .OrderByDescending(aa => aa.Created).ToListAsync();

                return View(adminActions);
            }
        }
    }
}