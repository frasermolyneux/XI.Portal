using System;

namespace XI.Portal.BLL.ViewModels
{
    public class PlayerListEntryViewModel
    {
        public Guid PlayerId { get; internal set; }
        public string Username { get; internal set; }
        public string Guid { get; internal set; }
        public string FirstSeen { get; internal set; }
        public string LastSeen { get; internal set; }
    }
}
