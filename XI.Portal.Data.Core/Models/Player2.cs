using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Core.Models
{
    public class Player2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid PlayerId { get; set; }

        [Index(IsUnique = false)]
        public GameType GameType { get; set; }

        public string Username { get; set; }

        public string Guid { get; set; }
        public string IpAddress { get; set; }

        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }

        public List<PlayerIpAddress> IpAddresses { get; set; }
    }
}