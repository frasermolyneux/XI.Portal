using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.BLL.Web.Interfaces;
using XI.Portal.BLL.Web.ViewModels;
using XI.Portal.Data.Contracts.FilterModels;
using XI.Portal.Data.Contracts.Repositories;

namespace XI.Portal.BLL.Web
{
    public class PlayersList : IPlayersList
    {
        private readonly IPlayersRepository playersRepository;

        public PlayersList(IPlayersRepository playersRepository)
        {
            this.playersRepository = playersRepository ?? throw new System.ArgumentNullException(nameof(playersRepository));
        }

        public async Task<int> GetPlayerListCount(PlayersFilterModel filterModel = null)
        {
            if (filterModel == null) filterModel = new PlayersFilterModel();

            return await playersRepository.GetPlayerCount(filterModel);
        }

        public async Task<List<PlayerListEntryViewModel>> GetPlayerList(PlayersFilterModel filterModel = null)
        {
            if (filterModel == null) filterModel = new PlayersFilterModel();

            var players = await playersRepository.GetPlayers(filterModel);

            var playerListEntryViewModels = players.Select(p => new PlayerListEntryViewModel
            {
                GameType = p.GameType.ToString(),
                PlayerId = p.PlayerId,
                Username = p.Username,
                Guid = p.Guid,
                IpAddress = p.IpAddress,
                FirstSeen = p.FirstSeen.ToString(CultureInfo.InvariantCulture),
                LastSeen = p.LastSeen.ToString(CultureInfo.InvariantCulture)
            }).ToList();

            return playerListEntryViewModels;
        }
    }
}
