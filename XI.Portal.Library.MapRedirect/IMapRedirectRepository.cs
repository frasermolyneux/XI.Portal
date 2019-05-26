using System.Collections.Generic;
using XI.Portal.Library.MapRedirect.Models;

namespace XI.Portal.Library.MapRedirect
{
    public interface IMapRedirectRepository
    {
        List<MapRedirectEntry> GetMapEntriesForGame(string game);
    }
}