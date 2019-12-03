using XI.Portal.Library.CommonTypes;

namespace XI.Portal.App.FileMonitorService.Interfaces
{
    internal interface IParserFactory
    {
        IParser GetParserForGameType(GameType gameType);
    }
}