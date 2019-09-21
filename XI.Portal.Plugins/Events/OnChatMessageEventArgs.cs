using System;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Plugins.Events
{
    public class OnChatMessageEventArgs : ServerBaseEventArgs
    {
        public OnChatMessageEventArgs(Guid serverId, GameType gameType, string guid, string name, string message, ChatType chatType) : base(serverId, gameType)
        {
            Guid = guid;
            Name = name;
            Message = message;
            ChatType = chatType;
        }

        public string Guid { get; }
        public string Name { get; }
        public string Message { get; }
        public ChatType ChatType { get; }
    }
}