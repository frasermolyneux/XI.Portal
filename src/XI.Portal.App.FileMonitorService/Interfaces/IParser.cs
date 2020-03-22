using System;
using XI.Portal.Data.CommonTypes;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.App.FileMonitorService.Interfaces
{
    public interface IParser : IPluginEvents
    {
        void ParseLine(Guid monitorId, string line, Guid serverId, string serverName, GameType gameType);
    }
}