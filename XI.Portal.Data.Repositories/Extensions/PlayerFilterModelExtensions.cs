using System.Data.Entity;
using System.Linq;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Repositories.Extensions
{
    public static class PlayerFilterModelExtensions
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
    }
}
