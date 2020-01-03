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

        public AnalyticsController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger,
            IAdminActionsAnalytics adminActionsAnalytics) : base(contextProvider, databaseLogger)
        {
            this.adminActionsAnalytics = adminActionsAnalytics ?? throw new ArgumentNullException(nameof(adminActionsAnalytics));
        }

        public async Task<ActionResult> AdminActions()
        {
            var adminActions = await adminActionsAnalytics.GetPastYearActionsGroupedByDate();

            return View(adminActions);
        }
    }
}