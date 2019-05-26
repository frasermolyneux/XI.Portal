using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class PlayerAlias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid PlayerAliasId { get; set; }

        [MaxLength(60)]
        [Index(IsUnique = false)]
        public string Name { get; set; }

        public DateTime Added { get; set; }
        public DateTime LastUsed { get; set; }
        public virtual Player2 Player { get; set; }
    }
}