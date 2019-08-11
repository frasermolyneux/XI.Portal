using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Auth.XtremeIdiots;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.SeniorAdmins)]
    public class AdmLogsController : BaseController
    {
        public AdmLogsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

        [HttpGet]
        public ActionResult UserLogs()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetUserLogsAjax(string sidx, string sord, int page, int rows)
        {
            using (var context = ContextProvider.GetContext())
            {
                var logs = context.UserLogs.OrderByDescending(l => l.Timestamp).AsQueryable();

                var totalRecords = logs.Count();
                var skip = (page - 1) * rows;

                var logList = await logs.Include(l => l.ApplicationUser).Skip(skip).Take(rows).ToListAsync();

                var logsToReturn = logList.Select(l => new
                {
                    Username = l.ApplicationUser?.UserName ?? "Anon",
                    l.Message,
                    Timestamp = l.Timestamp.ToString(CultureInfo.InvariantCulture)
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = logsToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult SystemLogs()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetSystemLogsAjax(string sidx, string sord, int page, int rows)
        {
            using (var context = ContextProvider.GetContext())
            {
                var logs = context.SystemLogs.OrderByDescending(l => l.Timestamp).AsQueryable();

                var totalRecords = logs.Count();
                var skip = (page - 1) * rows;

                var logList = await logs.Skip(skip).Take(rows).ToListAsync();

                var logsToReturn = logList.Select(l => new
                {
                    l.Level,
                    l.Message,
                    l.Error,
                    Timestamp = l.Timestamp.ToString(CultureInfo.InvariantCulture)
                });

                return Json(new
                {
                    total = totalRecords / rows,
                    page,
                    records = totalRecords,
                    rows = logsToReturn
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}