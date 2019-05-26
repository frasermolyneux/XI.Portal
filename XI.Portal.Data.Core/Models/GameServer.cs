using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Core.Models
{
    public class GameServer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid ServerId { get; set; }

        [MaxLength(60)]
        public string Title { get; set; }

        public GameType GameType { get; set; } = GameType.Unknown;

        public string Hostname { get; set; }
        public int QueryPort { get; set; }

        public string FtpHostname { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }

        public string RconPassword { get; set; }

        public string LiveTitle { get; set; }
        public string LiveMap { get; set; }
        public string LiveMod { get; set; }
        public int LiveMaxPlayers { get; set; }
        public int LiveCurrentPlayers { get; set; }
        public DateTime LiveLastUpdated { get; set; }

        public bool ShowOnBannerServerList { get; set; } = false;
        public int BannerServerListPosition { get; set; } = 0;
        public string HtmlBanner { get; set; }

        public bool ShowOnPortalServerList { get; set; }

        public bool ShowChatLog { get; set; }
    }
}