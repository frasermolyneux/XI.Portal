using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitorService.Interfaces
{
    internal interface IParserFactory
    {
        IParser GetParserForGameType(GameType gameType);
    }
}