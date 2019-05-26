using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class UserLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid UserLogId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public string Message { get; set; }

        [Index]
        public DateTime Timestamp { get; set; }
    }
}