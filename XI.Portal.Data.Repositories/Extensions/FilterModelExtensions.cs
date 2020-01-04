using System;
using System.Data.Entity;
using System.Linq;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Data.Repositories.Extensions
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
                            p.Guid.Contains(filterModel.FilterString) || 
                            p.Aliases.Any(a => a.Name.Contains(filterModel.FilterString)))
                            .AsQueryable();
                        break;
                    case PlayersFilterModel.FilterType.IpAddress:
                        players = players.Where(p => p.IpAddress.Contains(filterModel.FilterString) ||
                            p.IpAddresses.Any(ip => ip.Address.Contains(filterModel.FilterString)))
                            .AsQueryable();
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
            var adminActions = context.AdminActions.Include(aa => aa.Player).AsQueryable();

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

        public static IQueryable<Map> ApplyFilter(this MapsFilterModel filterModel, PortalContext context)
        {
            var maps = context.Maps.AsQueryable();

            if (filterModel.GameType != GameType.Unknown)
            {
                maps = maps.Where(m => m.GameType == filterModel.GameType).AsQueryable();
            }

            if (!string.IsNullOrWhiteSpace(filterModel.FilterString))
            {
                maps = maps.Where(m => m.MapName.Contains(filterModel.FilterString)).AsQueryable();
            }

            switch (filterModel.Order)
            {
                case MapsFilterModel.OrderBy.MapName:
                    maps = maps.OrderByDescending(m => m.MapName).AsQueryable();
                    break;
            }

            maps = maps.Skip(filterModel.SkipEntries).AsQueryable();

            if (filterModel.TakeEntries != 0)
            {
                maps = maps.Take(filterModel.TakeEntries).AsQueryable();
            }

            return maps;
        }

    }
}
