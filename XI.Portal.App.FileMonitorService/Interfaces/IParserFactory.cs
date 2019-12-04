using XI.Portal.Data.CommonTypes;

namespace XI.Portal.App.FileMonitorService.Interfaces
{
    internal interface IParserFactory
    {
        IParser GetParserForGameType(GameType gameType);
    }
}