using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class LivePlayerLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid LivePlayerLocationId { get; set; }

        public string IpAddress { get; set; }

        public double Lat { get; set; }
        public double Long { get; set; }

        public DateTime LastSeen { get; set; }
    }
}