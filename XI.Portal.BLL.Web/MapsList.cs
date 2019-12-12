using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Data.CommonTypes.Extensions;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;

namespace XI.Portal.BLL.Web
{
    public class MapsList : IMapsList
    {
        private readonly IMapsRepository mapsRepository;
        private readonly IMapsConfiguration mapsConfiguration;

        public MapsList(IMapsRepository mapsRepository, IMapsConfiguration mapsConfiguration)
        {
            this.mapsRepository = mapsRepository ?? throw new System.ArgumentNullException(nameof(mapsRepository));
            this.mapsConfiguration = mapsConfiguration ?? throw new System.ArgumentNullException(nameof(mapsConfiguration));
        }

        public async Task<int> GetMapListCount(MapsFilterModel filterModel)
        {
            if (filterModel == null) filterModel = new MapsFilterModel();

            return await mapsRepository.GetMapCount(filterModel);
        }

        public async Task<List<MapsListEntryViewModel>> GetMapList(MapsFilterModel filterModel)
        {
            if (filterModel == null) filterModel = new MapsFilterModel();

            var maps = await mapsRepository.GetMaps(filterModel);

            var mapsResult = new List<MapsListEntryViewModel>();

            foreach (var map in maps)
            {
                double totalLikes = map.MapVotes.Count(mv => mv.Like);
                double totalDislikes = map.MapVotes.Count(mv => !mv.Like);
                var totalVotes = map.MapVotes.Count();
                double likePercentage = 0;
                double dislikePercentage = 0;

                if (totalVotes > 0)
                {
                    likePercentage = (totalLikes / totalVotes) * 100;
                    dislikePercentage = (totalDislikes / totalVotes) * 100;
                }

                var mapListEntryViewModel = new MapsListEntryViewModel
                {
                    GameType = map.GameType.ToString(),
                    MapName = map.MapName,
                    TotalVotes = totalVotes,
                    TotalLikes = totalLikes,
                    TotalDislike = totalDislikes,
                    LikePercentage = likePercentage,
                    DislikePercentage = dislikePercentage,
                    MapFiles = new Dictionary<string, string>()
                };

                foreach (var mapFile in map.MapFiles)
                {
                    mapListEntryViewModel.MapFiles.Add(mapFile.FileName, $"{mapsConfiguration.MapRedirectBaseUrl}/redirect/{map.GameType.ToShortGameName()}/usermaps/{map.MapName}/{mapFile.FileName}");
                }

                mapsResult.Add(mapListEntryViewModel);
            }

            return mapsResult;
        }
    }
}