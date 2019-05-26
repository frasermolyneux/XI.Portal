using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Web.Portal.ViewModels.AdmServers
{
    public class EditGameServerViewModel
    {
        public Guid ServerId { get; set; }

        [MaxLength(60)]
        [Required(ErrorMessage = "A title is required for the server to be created")]
        public string Title { get; set; }

        public bool ShowOnBannerServerList { get; set; }

        public int ServerListPosition { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string HtmlBanner { get; set; }

        public GameType GameType { get; set; }

        public string Hostname { get; set; }
        public int QueryPort { get; set; }

        public bool ShowOnPortalServerList { get; set; }
        public string FtpHostname { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public string RconPassword { get; set; }

        public bool ShowChatLog { get; set; }
    }
}