using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.BLL.Interfaces;
using XI.Portal.BLL.ViewModels;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.BLL
{
    public class PlayersList : IPlayersList
    {
        private readonly IPlayersRepository playersRepository;

        public PlayersList(IPlayersRepository playersRepository)
        {
            this.playersRepository = playersRepository ?? throw new System.ArgumentNullException(nameof(playersRepository));
        }

        public async Task<int> GetPlayerListCount(GetPlayersFilterModel filterModel = null)
        {
            if (filterModel == null) filterModel = new GetPlayersFilterModel();

            return await playersRepository.GetPlayerCount(filterModel);
        }

        public async Task<List<PlayerListEntryViewModel>> GetPlayerList(GetPlayersFilterModel filterModel = null)
        {
            if (filterModel == null) filterModel = new GetPlayersFilterModel();

            var players = await playersRepository.GetPlayers(filterModel);

            var playerListEntryViewModels = players.Select(p => new PlayerListEntryViewModel
            {
                PlayerId = p.PlayerId,
                Username = p.Username,
                Guid = p.Guid,
                FirstSeen = p.FirstSeen.ToString(CultureInfo.InvariantCulture),
                LastSeen = p.LastSeen.ToString(CultureInfo.InvariantCulture)
            }).ToList();

            return playerListEntryViewModels;
        }
    }
}
