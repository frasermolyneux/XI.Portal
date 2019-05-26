using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XI.Portal.Library.CommonTypes;

namespace XI.Portal.Data.Core.Models
{
    public class ChatLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsUnique = true)]
        public Guid ChatLogId { get; set; }

        public GameServer GameServer { get; set; }

        public Player2 Player { get; set; }

        public string Username { get; set; }

        public ChatType ChatType { get; set; }

        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.MinValue;
    }
}