using System;

namespace XI.Portal.Plugins.Events
{
    public class OnMapRotationRconResponse : EventArgs
    {
        public Guid ServerId { get; set; }
        public string ResponseData { get; set; }
    }
}