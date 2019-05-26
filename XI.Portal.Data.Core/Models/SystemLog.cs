using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XI.Portal.Data.Core.Models
{
    public class SystemLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid SystemLogId { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }

        public string Error { get; set; }

        [Index]
        public DateTime Timestamp { get; set; }
    }
}