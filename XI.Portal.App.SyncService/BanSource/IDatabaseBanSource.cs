using System.Collections.Generic;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanSource
{
    public interface IDatabaseBanSource
    {
        IDictionary<string, string> GetBans(GameType gameType);
    }
}