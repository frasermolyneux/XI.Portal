using System.Collections.Generic;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.SyncService.BanSource
{
    public interface IExternalBanSource
    {
        ICollection<string> GetBans(GameType gameType);
    }
}