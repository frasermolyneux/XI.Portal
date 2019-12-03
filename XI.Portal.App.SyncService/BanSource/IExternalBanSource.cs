using System.Collections.Generic;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanSource
{
    public interface IExternalBanSource
    {
        ICollection<string> GetBans(GameType gameType);
    }
}