using System;

namespace XI.Portal.Plugins.Interfaces
{
    public interface IPluginEvents
    {
        //File Monitor Events
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

        //RconMonitorEvents
        event EventHandler MapRotationRconResponse; 
        event EventHandler StatusRconResponse;
    }
}