using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.Logging;
using XI.Portal.Library.MapRedirect;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMapsController : Controller
    {
        private readonly IContextProvider contextProvider;
        private readonly IDatabaseLogger databaseLogger;
        private readonly IMapRedirectRepository mapRedirectRepository;

        public AdmMapsController(IContextProvider contextProvider, IDatabaseLogger databaseLogger, IMapRedirectRepository mapRedirectRepository)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            this.databaseLogger = databaseLogger ?? throw new ArgumentNullException(nameof(databaseLogger));
            this.mapRedirectRepository = mapRedirectRepository ?? throw new ArgumentNullException(nameof(mapRedirectRepository));
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = contextProvider.GetContext())
            {
                var maps = await context.Maps.Include(map => map.MapFiles).ToListAsync();
                return View(maps);
            }
        }

        [HttpGet]
        public async Task<ActionResult> SyncWithRedirect()
        {
            var gamesToSync = new Dictionary<GameType, string>
            {
                {GameType.CallOfDuty4, "cod4"},
                {GameType.CallOfDuty5, "cod5"}
            };

            using (var context = contextProvider.GetContext())
            {
                await databaseLogger.CreateUserLogAsync(User.Identity.GetUserId(),
                    $"User has executed the maps sync process");

                var allMapFiles = await context.Maps.Include(m => m.MapFiles).ToListAsync();

                foreach (var game in gamesToSync)
                {
                    var maps = mapRedirectRepository.GetMapEntriesForGame(game.Value);

                    Parallel.ForEach(maps, mapRedirectEntry =>
                    {
                        var mapName = mapRedirectEntry.MapName;
                        var mapFiles = mapRedirectEntry.MapFiles
                            .Where(file => file.EndsWith(".iwd") | file.EndsWith(".ff"));

                        var map = allMapFiles.SingleOrDefault(m =>
                            m.MapName == mapName && m.GameType == game.Key);

                        if (map == null)
                        {
                            var mapToAdd = new Map
                            {
                                GameType = game.Key,
                                MapName = mapName,
                                MapFiles = mapFiles.Select(mf => new MapFile
                                {
                                    FileName = mf
                                }).ToList()
                            };

                            context.Maps.Add(mapToAdd);
                        }
                        else
                        {
                            map.MapFiles?.Clear();
                            map.MapFiles = mapFiles.Select(mf => new MapFile
                            {
                                FileName = mf
                            }).ToList();
                        }
                    });
                }

                await context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}