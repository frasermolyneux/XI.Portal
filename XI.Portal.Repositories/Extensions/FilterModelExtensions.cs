using System;
using System.Linq;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Repositories.Extensions
{
    public static class FilterModelExtensions
    {
        public static IQueryable<Player2> ApplyFilter(this PlayersFilterModel filterModel, PortalContext context)
        {
            var players = context.Players.AsQueryable();

            if (filterModel.GameType != GameType.Unknown)
            {
                players = players.Where(p => p.GameType == filterModel.GameType).AsQueryable();
            }

            if (filterModel.Filter != PlayersFilterModel.FilterType.None && !string.IsNullOrWhiteSpace(filterModel.FilterString))
            {
                switch (filterModel.Filter)
                {
                    case PlayersFilterModel.FilterType.UsernameAndGuid:
                        players = players.Where(p => p.Username.Contains(filterModel.FilterString) || 
                            p.Guid.Contains(filterModel.FilterString))
                            .AsQueryable();
                        break;
                    case PlayersFilterModel.FilterType.IpAddress:
                        players = players.Where(p => p.IpAddress.Contains(filterModel.FilterString) || 
                            p.IpAddresses.Any(ip => ip.Address.Contains(filterModel.FilterString)))
                            .AsQueryable();
                        break;
                    default:
                        break;
                }
            }
            else if (filterModel.Filter == PlayersFilterModel.FilterType.IpAddress)
            {
                players = players.Where(p => p.IpAddress != "" && p.IpAddress != null).AsQueryable();
            }

            switch (filterModel.Order)
            {
                case PlayersFilterModel.OrderBy.LastSeen:
                    players = players.OrderByDescending(p => p.LastSeen).AsQueryable();
                    break;
                case PlayersFilterModel.OrderBy.Username:
                    players = players.OrderBy(p => p.Username).AsQueryable();
                    break;
                default:
                    break;
            }

            players = players.Skip(filterModel.SkipPlayers).AsQueryable();

            if (filterModel.TakePlayers != 0)
            {
                players = players.Take(filterModel.TakePlayers).AsQueryable();
            }

            return players;
        }

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
                    adminActions = context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null
                        || aa.Type == AdminActionType.TempBan && aa.Expires > DateTime.UtcNow)
                        .AsQueryable();
                    break;
                case AdminActionsFilterModel.FilterType.UnclaimedBans:
                    adminActions = context.AdminActions.Where(aa => aa.Type == AdminActionType.Ban && aa.Expires == null 
                        && aa.Admin == null)
                        .AsQueryable();
                    break;
                default:
                    break;
            }

            switch (filterModel.Order)
            {
                case AdminActionsFilterModel.OrderBy.Created:
                    adminActions = adminActions.OrderByDescending(aa => aa.Created).AsQueryable();
                    break;
                default:
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
