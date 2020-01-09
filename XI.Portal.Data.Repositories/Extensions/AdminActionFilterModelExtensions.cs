using System;
using System.Data.Entity;
using System.Linq;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Repositories.Extensions
{
    public static class AdminActionFilterModelExtensions
    {
        public static IQueryable<AdminAction> ApplyFilter(this AdminActionsFilterModel filterModel, PortalContext context)
        {
            var adminActions = context.AdminActions.AsQueryable();

            if (filterModel.GameType != GameType.Unknown)
            {
                adminActions = adminActions.Where(aa => aa.Player.GameType == filterModel.GameType).AsQueryable();
            }

            switch (filterModel.Filter)
            {
                case AdminActionsFilterModel.FilterType.ActiveBans:
                    adminActions = adminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null
                                                            || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow)
                        .AsQueryable();
                    break;
                case AdminActionsFilterModel.FilterType.UnclaimedBans:
                    adminActions = adminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null
                                                                                           && aa.Admin == null)
                        .AsQueryable();
                    break;
            }

            switch (filterModel.Order)
            {
                case AdminActionsFilterModel.OrderBy.Created:
                    adminActions = adminActions.OrderBy(aa => aa.Created).AsQueryable();
                    break;
                case AdminActionsFilterModel.OrderBy.CreatedDesc:
                    adminActions = adminActions.OrderByDescending(aa => aa.Created).AsQueryable();
                    break;
            }

            adminActions = adminActions.Skip(filterModel.SkipEntries).AsQueryable();

            if (filterModel.TakeEntries != 0)
            {
                adminActions = adminActions.Take(filterModel.TakeEntries).AsQueryable();
            }

            return adminActions;
        }
    }
}
