using System.Collections.Generic;
using XI.Portal.Services.MapRedirect.Models;

namespace XI.Portal.Services.MapRedirect.Interfaces
{
    public interface IMapRedirectRepository
    {
        List<MapRedirectEntry> GetMapEntriesForGame(string game);
    }
}