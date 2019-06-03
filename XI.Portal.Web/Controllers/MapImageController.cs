using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Library.GameTracker;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class MapImageController : BaseController
    {
        private readonly IMapImageRepository mapImageRepository;

        public MapImageController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IMapImageRepository mapImageRepository) : base(contextProvider, databaseLogger)
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