using System.Text.RegularExpressions;
using XI.Portal.App.SyncService.Extensions;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.SyncService.BanFiles
{
    public class GuidValidator : IGuidValidator
    {
        public bool IsValid(GameType gameType, string guid)
        {
            return Regex.Match(guid, gameType.GuidRegex()).Success;
        }
    }
}