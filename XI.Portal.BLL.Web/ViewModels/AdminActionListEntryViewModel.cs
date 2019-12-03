using System;

namespace XI.Portal.BLL.Web.ViewModels
{
    public class AdminActionListEntryViewModel
    {
        public Guid PlayerId { get; internal set; }
        public string Username { get; internal set; }
        public string Guid { get; internal set; }
        public string Type { get; internal set; }
        public string Expires { get; internal set; }
    }
}
