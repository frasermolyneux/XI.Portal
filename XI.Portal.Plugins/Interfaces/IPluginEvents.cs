using System;

namespace XI.Portal.Plugins.Interfaces
{
    public interface IPluginEvents
    {
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