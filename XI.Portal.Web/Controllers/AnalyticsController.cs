using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Analytics.Interfaces;
using XI.Portal.Library.Logging;

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
            var adminActions = await adminActionsAnalytics.GetPastYearActionsGroupedByDate();

            return View(adminActions);
        }

        public async Task<ActionResult> Players()
        {
            var players = await playersAnalytics.GetCumulativeTrackedPlayers(DateTime.UtcNow.AddYears(-1));

            return View(players);
        }
    }
}