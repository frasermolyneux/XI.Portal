using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Library.MapRedirect.Models;

namespace XI.Portal.Library.MapRedirect
{
    public class MapRedirectRepository : IMapRedirectRepository
    {
        private readonly IMapsConfiguration mapRedirectConfiguration;

        public MapRedirectRepository(IMapsConfiguration mapRedirectConfiguration)
        {
            this.mapRedirectConfiguration = mapRedirectConfiguration ?? throw new ArgumentNullException(nameof(mapRedirectConfiguration));
        }

        public List<MapRedirectEntry> GetMapEntriesForGame(string game)
        {
            using (var client = new WebClient())
            {
                var content = client.DownloadString($"{mapRedirectConfiguration.MapRedirectBaseUrl}/portal-map-sync.php?game={game}&key={mapRedirectConfiguration.MapRedirectKey}");
                return JsonConvert.DeserializeObject<List<MapRedirectEntry>>(content);
            }
        }
    }
}