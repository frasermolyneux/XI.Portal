using System;
using Serilog;
using XI.Portal.Plugins.Events;
using XI.Portal.Services.FileMonitorService.Interfaces;

namespace XI.Portal.Services.FileMonitorService.Parsers
{
    public class BaseParser : IParser
    {
        public readonly ILogger Logger;

        public BaseParser(ILogger logger)
        {
            Logger = logger;
        }

        public virtual void ParseLine(string line, Guid serverId)
        {
        }

        public event EventHandler LineRead;
        public event EventHandler PlayerConnected;
        public event EventHandler PlayerDisconnected;
        public event EventHandler ChatMessage;
        public event EventHandler Kill;
        public event EventHandler TeamKill;
        public event EventHandler Suicide;
        public event EventHandler RoundStart;
        public event EventHandler Action;
        public event EventHandler Damage;

        protected virtual void OnLineRead(LineReadEventArgs eventArgs)
        {
            LineRead?.Invoke(this, eventArgs);
        }

        protected virtual void OnPlayerConnected(OnPlayerConnectedEventArgs eventArgs)
        {
            PlayerConnected?.Invoke(this, eventArgs);
        }

        protected virtual void OnPlayerDisconnected(OnPlayerDisconnectedEventArgs eventArgs)
        {
            PlayerDisconnected?.Invoke(this, eventArgs);
        }

        protected virtual void OnChatMessage(OnChatMessageEventArgs eventArgs)
        {
            ChatMessage?.Invoke(this, eventArgs);
        }

        protected virtual void OnKill(OnKillEventArgs eventArgs)
        {
            Kill?.Invoke(this, eventArgs);
        }

        protected virtual void OnTeamKill(OnTeamKillEventArgs eventArgs)
        {
            TeamKill?.Invoke(this, eventArgs);
        }

        protected virtual void OnSuicide(OnSuicideEventArgs eventArgs)
        {
            Suicide?.Invoke(this, eventArgs);
        }

        protected virtual void OnRoundStart(OnRoundStartEventArgs eventArgs)
        {
            RoundStart?.Invoke(this, eventArgs);
        }

        protected virtual void OnAction(OnActionEventArgs eventArgs)
        {
            Action?.Invoke(this, eventArgs);
        }

        protected virtual void OnDamage(OnDamageEventArgs eventArgs)
        {
            Damage?.Invoke(this, eventArgs);
        }
    }
}