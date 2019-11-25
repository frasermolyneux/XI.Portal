using System.Collections.Generic;
using System.Threading.Tasks;
using XI.Portal.BLL.Interfaces;
using XI.Portal.BLL.ViewModels;
using XI.Portal.Repositories.Interfaces;

namespace XI.Portal.BLL.PlayerManagement
{
    public class PortalIndex : IPortalIndex
    {
        private readonly IPlayersRepository playersRepository;
        private readonly ILivePlayersRepository livePlayersRepository;
        private readonly IAdminActionsRepositories adminActionsRepository;

        public PortalIndex(IPlayersRepository playersRepository,
            ILivePlayersRepository livePlayersRepository,
            IAdminActionsRepositories adminActionsRepository)
        {
            this.playersRepository = playersRepository ?? throw new System.ArgumentNullException(nameof(playersRepository));
            this.livePlayersRepository = livePlayersRepository ?? throw new System.ArgumentNullException(nameof(livePlayersRepository));
            this.adminActionsRepository = adminActionsRepository ?? throw new System.ArgumentNullException(nameof(adminActionsRepository));
        }

        public async Task<PortalIndexViewModel> GetPortalIndexViewModel()
        {
            var portalIndexViewModel = new PortalIndexViewModel
            {
                TrackedPlayerCount = await playersRepository.GetTrackedPlayerCount(),
                OnlinePlayerCount = await livePlayersRepository.GetOnlinePlayerCount(),
                ActiveBanCount = await adminActionsRepository.GetActiveBansCount(),
                UnclaimedBanCount = await adminActionsRepository.GetUnclaimedBansCount(),
                GameIndexViewModels = new List<GameIndexViewModel>()
            };

            var playerGames = await playersRepository.GetPlayerGames();

            foreach (var playerGame in playerGames)
            {
                var gameIndexViewModel = new GameIndexViewModel
                {
                    GameType = playerGame,
                    TrackedPlayerCount = await playersRepository.GetTrackedPlayerCount(playerGame),
                    ActiveBanCount = await adminActionsRepository.GetActiveBansCount(playerGame)
                };

                portalIndexViewModel.GameIndexViewModels.Add(gameIndexViewModel);
            }

            return portalIndexViewModel;
        }
    }
}
