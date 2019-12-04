using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using XI.Portal.Configuration.Interfaces;
using XI.Portal.Services.MapRedirect.Interfaces;
using XI.Portal.Services.MapRedirect.Models;

namespace XI.Portal.Services.MapRedirect.Repositories
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