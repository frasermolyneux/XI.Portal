using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Contracts.Models;
using XI.Portal.BLL.Interfaces;
using XI.Portal.BLL.ViewModels;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.BLL
{
    public class PortalIndex : IPortalIndex
    {
        private readonly IPlayersRepository playersRepository;
        private readonly ILivePlayersRepository livePlayersRepository;
        private readonly IAdminActionsRepository adminActionsRepository;

        public PortalIndex(IPlayersRepository playersRepository,
            ILivePlayersRepository livePlayersRepository,
            IAdminActionsRepository adminActionsRepository)
        {
            this.playersRepository = playersRepository ?? throw new System.ArgumentNullException(nameof(playersRepository));
            this.livePlayersRepository = livePlayersRepository ?? throw new System.ArgumentNullException(nameof(livePlayersRepository));
            this.adminActionsRepository = adminActionsRepository ?? throw new System.ArgumentNullException(nameof(adminActionsRepository));
        }

        public async Task<PortalIndexViewModel> GetPortalIndexViewModel()
        {
            var portalIndexViewModel = new PortalIndexViewModel
            {
                TrackedPlayerCount = await playersRepository.GetPlayerCount(new PlayersFilterModel()),
                OnlinePlayerCount = await livePlayersRepository.GetOnlinePlayerCount(),
                ActiveBanCount = await adminActionsRepository.GetAdminActionsCount(new AdminActionsFilterModel
                {
                    Filter = AdminActionsFilterModel.FilterType.ActiveBans
                }),
                UnclaimedBanCount = await adminActionsRepository.GetAdminActionsCount(new AdminActionsFilterModel
                {
                    Filter = AdminActionsFilterModel.FilterType.UnclaimedBans
                }),
                GameIndexViewModels = new List<GameIndexViewModel>()
            };

            var playerGames = await playersRepository.GetPlayerGames();

            foreach (var playerGame in playerGames)
            {
                var gameIndexViewModel = new GameIndexViewModel
                {
                    GameType = playerGame,
                    TrackedPlayerCount = await playersRepository.GetPlayerCount(new PlayersFilterModel
                    {
                        GameType = playerGame
                    }),
                    ActiveBanCount = await adminActionsRepository.GetAdminActionsCount(new AdminActionsFilterModel
                    {
                        GameType = playerGame,
                        Filter = AdminActionsFilterModel.FilterType.ActiveBans
                    })
                };

                portalIndexViewModel.GameIndexViewModels.Add(gameIndexViewModel);
            }

            return portalIndexViewModel;
        }
    }
}
