﻿using System.Collections.Generic;
using Serilog;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.ChatMonitorPlugin;
using XI.Portal.Plugins.FuckYouPlugin;
using XI.Portal.Plugins.LogProxyPlugin;
using XI.Portal.Plugins.PlayerInfoPlugin;
using XI.Portal.Services.FileMonitorService.Interfaces;
using XI.Portal.Services.FileMonitorService.Parsers;

namespace XI.Portal.Services.FileMonitorService.Factories
{
    internal class ParserFactory : IParserFactory
    {
        public ParserFactory(ILogger logger, ChatMonitorPlugin chatMonitorPlugin, PlayerInfoPlugin playerInfoPlugin, FuckYouPlugin fuckYouPlugin, LogProxyPlugin logProxyPlugin)
        {
            Parsers.Add(GameType.CallOfDuty2, new Cod2Parser(logger));
            Parsers.Add(GameType.CallOfDuty4, new Cod4Parser(logger));
            Parsers.Add(GameType.CallOfDuty5, new Cod5Parser(logger));

            foreach (var parser in Parsers)
            {
                chatMonitorPlugin.RegisterEventHandlers(parser.Value);
                playerInfoPlugin.RegisterEventHandlers(parser.Value);
                fuckYouPlugin.RegisterEventHandlers(parser.Value);
                logProxyPlugin.RegisterEventHandlers(parser.Value);
            }
        }

        private Dictionary<GameType, IParser> Parsers { get; } = new Dictionary<GameType, IParser>();

        public IParser GetParserForGameType(GameType gameType)
        {
            return Parsers[gameType];
        }
    }
}