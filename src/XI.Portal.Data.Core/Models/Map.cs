using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XI.Portal.Data.CommonTypes;

namespace XI.Portal.Data.Core.Models
{
    public class Map
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid MapId { get; set; }

        public GameType GameType { get; set; }

        public string MapName { get; set; }

        public List<MapFile> MapFiles { get; set; }

        public List<MapVote> MapVotes { get; set; }
    }
}