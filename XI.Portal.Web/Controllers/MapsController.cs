using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.Logging;

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
    }
}