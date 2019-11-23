using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class MapsController : BaseController
    {
        public MapsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
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
            using (var context = ContextProvider.GetContext())
            {
                var maps = context.Maps.Include(m => m.MapVotes).OrderBy(m => m.MapName).AsQueryable();

                if (_search && !string.IsNullOrWhiteSpace(searchString))
                {
                    maps = maps.Where(cl => cl.MapName.Contains(searchString)).AsQueryable();
                }

                var totalRecords = maps.Count();
                var skip = (page - 1) * rows;

                var mapList = await maps.Skip(skip).Take(rows).ToListAsync();

                var mapsToReturn = mapList.Select(map =>
                {
                    double totalLikes = map.MapVotes.Where(mv => mv.Like).Count();
                    double totalDislikes = map.MapVotes.Where(mv => !mv.Like).Count();
                    int totalVotes = map.MapVotes.Count();
                    double likePercentage = 0;
                    double dislikePercentage = 0;

                    if (totalVotes > 0)
                    {
                        likePercentage = (totalLikes / totalVotes) * 100;
                        dislikePercentage = (totalDislikes / totalVotes) * 100;
                    }

                    return new
                    {
                        GameType = map.GameType.ToString(),
                        map.MapName,
                        TotalVotes = totalVotes,
                        TotalLike = totalLikes,
                        TotalDislike = totalDislikes,
                        LikePercentage = likePercentage,
                        DislikePercentage = dislikePercentage,
                    };
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = mapsToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}