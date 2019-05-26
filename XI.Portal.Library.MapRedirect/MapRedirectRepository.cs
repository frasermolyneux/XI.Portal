using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using XI.Portal.Library.MapRedirect.Models;

namespace XI.Portal.Library.MapRedirect
{
    public class MapRedirectRepository : IMapRedirectRepository
    {
        public List<MapRedirectEntry> GetMapEntriesForGame(string game)
        {
            using (var client = new WebClient())
            {
                var content = client.DownloadString("http://redirect.xtremeidiots.net/portal-map-sync.php?game=COD4");
                return JsonConvert.DeserializeObject<List<MapRedirectEntry>>(content);
            }
        }
    }
}