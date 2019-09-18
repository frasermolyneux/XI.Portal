using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Services.FileMonitor.Interfaces
{
    internal interface IParserFactory
    {
        IParser GetParserForGameType(GameType gameType);
    }
}