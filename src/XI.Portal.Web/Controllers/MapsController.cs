using System;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;
using XI.Portal.Web.Extensions;

namespace XI.Portal.Web.Controllers
{
    [AllowAnonymous]
    public class MapsController : BaseController
    {
        private readonly IMapsList mapList;

        public MapsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IMapsList mapList) : base(contextProvider, databaseLogger)
        {
            this.mapList = mapList ?? throw new System.ArgumentNullException(nameof(mapList));
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetGlobalMapListAjax(string sidx, string sord, int page, int rows, string searchString)
        {
            var mapsFilterModel = new MapsFilterModel
            {
                FilterString = searchString
            };

            var mapListCount = await mapList.GetMapListCount(mapsFilterModel);

            mapsFilterModel.SkipEntries = (page - 1) * rows;
            mapsFilterModel.TakeEntries = rows;

            var searchColumn = string.IsNullOrWhiteSpace(sidx) ? "MapName" : sidx;
            var searchOrder = string.IsNullOrWhiteSpace(sord) ? "asc" : sord;

            switch (searchColumn)
            {
                case "MapName":
                    mapsFilterModel.Order = searchOrder == "asc" ? MapsFilterModel.OrderBy.MapNameAsc : MapsFilterModel.OrderBy.MapNameDesc;
                    break;
                case "LikeDislike":
                    mapsFilterModel.Order = searchOrder == "asc" ? MapsFilterModel.OrderBy.LikeDislikeAsc : MapsFilterModel.OrderBy.LikeDislikeDesc;
                    break;
                case "GameType":
                    mapsFilterModel.Order = searchOrder == "asc" ? MapsFilterModel.OrderBy.GameTypeAsc : MapsFilterModel.OrderBy.GameTypeDesc;
                    break;
                default:
                    mapsFilterModel.Order = searchOrder == "asc" ? MapsFilterModel.OrderBy.MapNameAsc : MapsFilterModel.OrderBy.MapNameDesc;
                    break;
            }

            var mapListEntries = await mapList.GetMapList(mapsFilterModel);

            return Json(new
            {
                total = mapListCount / rows,
                page,
                records = mapListCount,
                rows = mapListEntries
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
        public async Task<ActionResult> DownloadFullRotation(string id)
        {
            if (!Guid.TryParse(id, out var idAsGuid))
                return RedirectToAction("Index");

            using (var context = ContextProvider.GetContext())
            {
                var gameServer = await context.GameServers
                    .SingleOrDefaultAsync(server => server.ServerId == idAsGuid);

                if (gameServer == null)
                    return RedirectToAction("Index");

                var mapRotation = await context.MapRotations
                    .Include(m => m.Map)
                    .Include(m => m.Map.MapFiles)
                    .Include(m => m.Map.MapVotes)
                    .Where(m => m.GameServer.ServerId == gameServer.ServerId).ToListAsync();

                var tempDirectory = GetTemporaryDirectory();

                using (var client = new WebClient())
                {
                    foreach (var mapEntry in mapRotation)
                    {
                        foreach (var mapFile in mapEntry.Map.MapFiles)
                        {
                            var dir = Path.Combine(tempDirectory, mapEntry.Map.MapName);

                            Directory.CreateDirectory(dir);

                            client.DownloadFile(new Uri($"{mapEntry.GameServer.GameType.GameRedirectBaseUrl()}/{mapEntry.Map.MapName}/{mapFile.FileName}"),
                                Path.Combine(dir, mapFile.FileName));
                        }
                    }
                }

                var zippedMapPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

                ZipFile.CreateFromDirectory(tempDirectory, zippedMapPath);

                return File(zippedMapPath, "application/zip");
            }
        }

        public string GetTemporaryDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }

}