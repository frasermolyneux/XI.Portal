using System;
using XI.Portal.Library.CommonTypes;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Services.FileMonitorService.Interfaces
{
    public interface IParser : IPluginEvents
    {
        void ParseLine(string line, Guid serverId, string serverName, GameType gameType);
    }
}