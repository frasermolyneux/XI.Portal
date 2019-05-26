using System;
using System.Web.Mvc;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.GameTracker;

namespace XI.Portal.Web.Portal.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class MapImageController : Controller
    {
        private readonly IMapImageRepository mapImageRepository;

        public MapImageController(IMapImageRepository mapImageRepository)
        {
            this.mapImageRepository = mapImageRepository ?? throw new ArgumentNullException(nameof(mapImageRepository));
        }

        [HttpGet]
        public ActionResult Image(GameType gameType, string mapName)
        {
            var mapImagePath = mapImageRepository.GetMapFilePath(gameType, mapName);
            return File(mapImagePath, "image/jpeg");
        }
    }
}