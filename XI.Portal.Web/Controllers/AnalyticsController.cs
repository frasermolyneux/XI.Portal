using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Analytics.Interfaces;
using XI.Portal.Library.Logging;
using XI.Portal.Web.ViewModels.Analytics;

namespace XI.Portal.Web.Controllers
{
    public class AnalyticsController : BaseController
    {
        private readonly IAdminActionsAnalytics adminActionsAnalytics;
        private readonly IPlayersAnalytics playersAnalytics;

        public AnalyticsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IAdminActionsAnalytics adminActionsAnalytics, 
            IPlayersAnalytics playersAnalytics) : base(contextProvider, databaseLogger)
        {
            this.adminActionsAnalytics = adminActionsAnalytics ?? throw new ArgumentNullException(nameof(adminActionsAnalytics));
            this.playersAnalytics = playersAnalytics ?? throw new ArgumentNullException(nameof(playersAnalytics));
        }

        public async Task<ActionResult> AdminActions()
        {
            var adminActions = await adminActionsAnalytics.GetDailyActions(DateTime.UtcNow.AddYears(-1));

            return View(adminActions);
        }

        public async Task<ActionResult> Players()
        {
            var cutoff = DateTime.UtcNow.AddYears(-1);
            ViewBag.DateFilterRange = cutoff;

            return View();
        }

        public async Task<ActionResult> GetCumulativeDailyPlayersJson(DateTime cutoff)
        {
            var data = await playersAnalytics.GetCumulativeDailyPlayers(cutoff);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetNewDailyPlayersPerGameJson(DateTime cutoff)
        {
            var data = await playersAnalytics.GetNewDailyPlayersPerGame(cutoff);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}