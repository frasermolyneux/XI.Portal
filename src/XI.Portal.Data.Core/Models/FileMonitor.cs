using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class FileMonitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid FileMonitorId { get; set; }

        public GameServer GameServer { get; set; }

        public string FilePath { get; set; }

        public long BytesRead { get; set; }

        public DateTime LastRead { get; set; }

        public string LastError { get; set; }
    }
}