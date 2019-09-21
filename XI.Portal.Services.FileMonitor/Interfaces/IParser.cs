using System;
using XI.Portal.Plugins.Interfaces;

namespace XI.Portal.Services.FileMonitor.Interfaces
{
    public interface IParser : IPluginEvents
    {
        void ParseLine(string line, Guid serverId);
    }
}