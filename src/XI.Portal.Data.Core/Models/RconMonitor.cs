using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class RconMonitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid RconMonitorId { get; set; }

        public GameServer GameServer { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool MonitorMapRotation { get; set; } = false;
        public DateTime MapRotationLastUpdated { get; set; }

        public bool MonitorPlayers { get; set; } = false;
        public bool MonitorPlayerIPs { get; set; } = false;

        public string LastError { get; set; }
    }
}