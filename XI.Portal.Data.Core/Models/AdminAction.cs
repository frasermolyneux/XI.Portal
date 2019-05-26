using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Core.Models
{
    public class AdminAction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid AdminActionId { get; set; }

        public Player2 Player { get; set; }

        public ApplicationUser Admin { get; set; } = null;

        public AdminActionType Type { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Expires { get; set; } = null;

        public int ForumTopicId { get; set; } = 0;
    }
}