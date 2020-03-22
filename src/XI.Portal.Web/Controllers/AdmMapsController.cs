using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmMapsController : BaseController
    {
        public AdmMapsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            using (var context = ContextProvider.GetContext())
            {
                var maps = await context.Maps.Include(map => map.MapFiles).ToListAsync();
                return View(maps);
            }
        }
    }
}