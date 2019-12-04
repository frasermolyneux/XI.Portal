using System.Collections.Generic;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.SyncService.BanSource
{
    public interface IExternalBanSource
    {
        ICollection<string> GetBans(GameType gameType);
    }
}