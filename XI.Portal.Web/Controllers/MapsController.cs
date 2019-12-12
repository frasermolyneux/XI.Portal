using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
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
        public async Task<ActionResult> GetGlobalMapListAjax(string sidx, string sord, int page, int rows,
        // ReSharper disable once InconsistentNaming
        bool _search, string searchField, string searchString, string searchOper)
        {
            var mapListCount = await mapList.GetMapListCount(new MapsFilterModel
            {
                FilterString = searchString
            });
            var mapsToSkip = (page - 1) * rows;

            var mapListEntries = await mapList.GetMapList(new MapsFilterModel
            {
                FilterString = searchString,
                SkipEntries = mapsToSkip,
                TakeEntries = rows
            });

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