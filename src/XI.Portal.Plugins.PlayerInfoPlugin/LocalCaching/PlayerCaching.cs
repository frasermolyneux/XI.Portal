using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Plugins.PlayerInfoPlugin.Interfaces;

namespace XI.Portal.Plugins.PlayerInfoPlugin.LocalCaching
{
    public class PlayerCaching : IPlayerCaching
    {
        private readonly ILogger logger;

        public PlayerCaching(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private Dictionary<string, DateTime> CachedPlayers { get; } = new Dictionary<string, DateTime>();

        public void AddToCache(GameType gameType, string guid, string name)
        {
            CachedPlayers.Add($"{gameType}{guid}{name}", DateTime.UtcNow);
        }

        public bool PlayerInCache(GameType gameType, string guid, string name)
        {
            return CachedPlayers.ContainsKey($"{gameType}{guid}{name}");
        }

        public void ReduceCache()
        {
            var expiredCachedPlayers = CachedPlayers.Where(ca => ca.Value < DateTime.UtcNow.AddMinutes(-15)).ToList();
            foreach (var cachedPlayer in expiredCachedPlayers)
            {
                logger.Debug("Clearing {player} from local cache as it has expired", cachedPlayer.Key);
                CachedPlayers.Remove(cachedPlayer.Key);
            }
        }
    }
}
