using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class BanFileMonitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid BanFileMonitorId { get; set; }

        public GameServer GameServer { get; set; }

        public string FilePath { get; set; }

        public long RemoteFileSize { get; set; }

        public DateTime LastSync { get; set; }

        public string LastError { get; set; }
    }
}