using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Analytics.Interfaces;
using XI.Portal.Library.Analytics.Models;

namespace XI.Portal.Library.Analytics.Providers
{
    public class AdminActionsAnalytics : IAdminActionsAnalytics
    {
        private readonly IContextProvider contextProvider;

        public AdminActionsAnalytics(IContextProvider contextProvider)
        {
            this.contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
        }


        public async Task<List<AdminActionAnalyticEntry>> GetDailyActions(DateTime cutoff)
        {
            using (var context = contextProvider.GetContext())
            {
                var adminActions = await context.AdminActions
                    .Where(aa => aa.Created > cutoff)
                    .Select(aa => aa.Created).ToListAsync();

                var groupedActions = adminActions.GroupBy(aa => new DateTime(aa.Year, aa.Month, aa.Day))
                    .Select(g => new AdminActionAnalyticEntry
                    {
                        Created = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(o => o.Created)
                    .ToList();

                return groupedActions;
            }
        }
    }
}
