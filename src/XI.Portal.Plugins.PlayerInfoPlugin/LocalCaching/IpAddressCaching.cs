using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using XI.Portal.Plugins.PlayerInfoPlugin.Interfaces;

namespace XI.Portal.Plugins.PlayerInfoPlugin.LocalCaching
{
    public class IpAddressCaching : IIpAddressCaching
    {
        private readonly ILogger logger;

        public IpAddressCaching(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private Dictionary<string, DateTime> CachedAddresses { get; } = new Dictionary<string, DateTime>();

        public void ReduceCache()
        {
            var expiredCachedAddresses = CachedAddresses.Where(ca => ca.Value < DateTime.UtcNow.AddHours(-1)).ToList();
            foreach (var cachedAddress in expiredCachedAddresses)
            {
                logger.Debug("Clearing {address} from local cache as it has expired", cachedAddress.Key);
                CachedAddresses.Remove(cachedAddress.Key);
            }
        }

        public bool IpAddressInCache(string ipAddress)
        {
            return CachedAddresses.ContainsKey(ipAddress);
        }

        public void AddToCache(string ipAddress)
        {
            CachedAddresses.Add(ipAddress, DateTime.UtcNow);
        }
    }
}