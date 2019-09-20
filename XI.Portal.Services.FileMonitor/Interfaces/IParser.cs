using System;

namespace XI.Portal.Services.FileMonitor.Interfaces
{
    public interface IParser
    {
        void ParseLine(string line, Guid serverId);

        event EventHandler LineRead;
        event EventHandler PlayerConnected;
        event EventHandler PlayerDisconnected;
        event EventHandler ChatMessage;
        event EventHandler Kill;
        event EventHandler TeamKill;
        event EventHandler Suicide;
        event EventHandler RoundStart;
        event EventHandler Action;
        event EventHandler Damage;
    }
}