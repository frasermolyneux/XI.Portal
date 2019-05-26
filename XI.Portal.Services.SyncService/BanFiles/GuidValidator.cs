using System.Text.RegularExpressions;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Services.SyncService.Extensions;

namespace XI.Portal.Services.SyncService.BanFiles
{
    public class GuidValidator : IGuidValidator
    {
        public bool IsValid(GameType gameType, string guid)
        {
            return Regex.Match(guid, gameType.GuidRegex()).Success;
        }
    }
}