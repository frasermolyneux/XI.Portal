using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Data.CommonTypes;
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
        private readonly IPortalIndex portalIndex;
        private readonly IPlayersList playersList;
        private readonly IAdminActionsList adminActionList;

        public PlayersController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IGeoLocationApiRepository geoLocationApiRepository, 
            IPortalIndex portalIndex,
            IPlayersList playersList,
            IAdminActionsList adminActionList) : base(contextProvider, databaseLogger)
        {
            this.geoLocationApiRepository = geoLocationApiRepository ?? throw new ArgumentNullException(nameof(geoLocationApiRepository));
            this.portalIndex = portalIndex ?? throw new ArgumentNullException(nameof(portalIndex));
            this.playersList = playersList ?? throw new ArgumentNullException(nameof(playersList));
            this.adminActionList = adminActionList ?? throw new ArgumentNullException(nameof(adminActionList));
        }

        [HttpGet]
        public async Task<ActionResult> Home()
        {
            return View(await portalIndex.GetPortalIndexViewModel());
        }

        [HttpGet]
        public ActionResult Index(GameType id)
        {
            return View(id);
        }

        [HttpGet]
        public async Task<ActionResult> GetPlayersAjax(GameType id, string sidx, string sord, int page, int rows, string searchString)
        {
            var playersFilterModel = new PlayersFilterModel
            {
                GameType = id,
                Filter = PlayersFilterModel.FilterType.UsernameAndGuid,
                FilterString = searchString
            };

            var playerListCount = await playersList.GetPlayerListCount(playersFilterModel);

            playersFilterModel.SkipPlayers = (page - 1) * rows;
            playersFilterModel.TakePlayers = rows;

            var searchColumn = string.IsNullOrWhiteSpace(sidx) ? "Username" : sidx;
            var searchOrder = string.IsNullOrWhiteSpace(sord) ? "asc" : sord;

            switch (searchColumn)
            {
                case "Username":
                    playersFilterModel.Order = searchOrder == "asc" ? PlayersFilterModel.OrderBy.UsernameAsc : PlayersFilterModel.OrderBy.UsernameDesc;
                    break;
                case "FirstSeen":
                    playersFilterModel.Order = searchOrder == "asc" ? PlayersFilterModel.OrderBy.FirstSeenAsc : PlayersFilterModel.OrderBy.FirstSeenDesc;
                    break;
                case "LastSeen":
                    playersFilterModel.Order = searchOrder == "asc" ? PlayersFilterModel.OrderBy.LastSeenAsc : PlayersFilterModel.OrderBy.LastSeenDesc;
                    break;
            }

            var playersListEntries = await playersList.GetPlayerList(playersFilterModel);

            return Json(new
            {
                total = playerListCount / rows,
                page,
                records = playerListCount,
                rows = playersListEntries
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IPSearch()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetPlayersIPSearchAjax(string sidx, string sord, int page, int rows, string searchString)
        {
            var playersFilterModel = new PlayersFilterModel
            {
                Filter = PlayersFilterModel.FilterType.IpAddress,
                FilterString = searchString
            };

            var playerListCount = await playersList.GetPlayerListCount(playersFilterModel);

            playersFilterModel.SkipPlayers = (page - 1) * rows;
            playersFilterModel.TakePlayers = rows;

            var searchColumn = string.IsNullOrWhiteSpace(sidx) ? "Username" : sidx;
            var searchOrder = string.IsNullOrWhiteSpace(sord) ? "asc" : sord;

            switch (searchColumn)
            {
                case "Username":
                    playersFilterModel.Order = searchOrder == "asc" ? PlayersFilterModel.OrderBy.UsernameAsc : PlayersFilterModel.OrderBy.UsernameDesc;
                    break;
            }

            var playersListEntries = await playersList.GetPlayerList(playersFilterModel);

            return Json(new
            {
                total = playerListCount / rows,
                page,
                records = playerListCount,
                rows = playersListEntries
            }, JsonRequestBehavior.AllowGet);
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
            var adminActionsListCount = await adminActionList.GetAdminActionsListCount(new AdminActionsFilterModel
            {
                GameType = id,
                Filter = AdminActionsFilterModel.FilterType.ActiveBans
            });
            var adminActionsToSkip = (page - 1) * rows;

            var adminActionsListEntry = await adminActionList.GetAdminActionsList(new AdminActionsFilterModel
            {
                GameType = id,
                Filter = AdminActionsFilterModel.FilterType.ActiveBans,
                SkipEntries = adminActionsToSkip,
                TakeEntries = rows
            });

            return Json(new
            {
                total = adminActionsListCount / rows,
                page,
                records = adminActionsListCount,
                rows = adminActionsListEntry
            }, JsonRequestBehavior.AllowGet);
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