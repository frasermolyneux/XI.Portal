using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class MapRotation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid MapRotationId { get; set; }

        public GameServer GameServer { get; set; }

        public Map Map { get; set; }

        public string GameMode { get; set; }
    }
}