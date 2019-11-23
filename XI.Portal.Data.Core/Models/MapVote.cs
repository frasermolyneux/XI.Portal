using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class MapVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid MapVoteId { get; set; }

        public Player2 Player { get; set; }
        public Map Map { get; set; }
        public bool Like { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
