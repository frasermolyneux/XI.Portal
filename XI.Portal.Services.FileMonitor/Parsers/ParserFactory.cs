using System.Collections.Generic;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Services.FileMonitor.Interfaces;
using XI.Portal.Services.FileMonitor.Plugins;

namespace XI.Portal.Services.FileMonitor.Parsers
{
    internal class ParserFactory : IParserFactory
    {
        public ParserFactory(ILogger logger, ChatMonitorPlugin chatMonitorPlugin, PlayerCorePlugin playerCorePlugin, FuckYouPlugin fuckYouPlugin, StatsLogProxyPlugin statsLogProxyPlugin)
        {
            Parsers.Add(GameType.CallOfDuty2, new Cod2Parser(logger));
            Parsers.Add(GameType.CallOfDuty4, new Cod4Parser(logger));
            Parsers.Add(GameType.CallOfDuty5, new Cod5Parser(logger));

            foreach (var parser in Parsers)
            {
                chatMonitorPlugin.RegisterEventHandlers(parser.Value);
                playerCorePlugin.RegisterEventHandlers(parser.Value);
                fuckYouPlugin.RegisterEventHandlers(parser.Value);
                statsLogProxyPlugin.RegisterEventHandlers(parser.Value);
            }
        }

        private Dictionary<GameType, IParser> Parsers { get; } = new Dictionary<GameType, IParser>();

        public IParser GetParserForGameType(GameType gameType)
        {
            return Parsers[gameType];
        }
    }
}