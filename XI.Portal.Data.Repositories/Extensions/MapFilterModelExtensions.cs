using System;
using System.Data.Entity;
using System.Linq;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Core.Context;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Data.Repositories.Extensions
{
    public static class MapFilterModelExtensions
    {
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
                case MapsFilterModel.OrderBy.MapNameAsc:
                    maps = maps.OrderBy(m => m.MapName).AsQueryable();
                    break;
                case MapsFilterModel.OrderBy.MapNameDesc:
                    maps = maps.OrderByDescending(m => m.MapName).AsQueryable();
                    break;
                case MapsFilterModel.OrderBy.LikeDislikeAsc:
                    maps = maps.Where(m => m.MapVotes.Any()).OrderBy(m => m.MapVotes.Count(mv => mv.Like)).AsQueryable();
                    break;
                case MapsFilterModel.OrderBy.LikeDislikeDesc:
                    maps = maps.Where(m => m.MapVotes.Any()).OrderByDescending(m => m.MapVotes.Count(mv => mv.Like)).AsQueryable();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
