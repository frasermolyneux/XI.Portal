using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.TempBans
{
    public class EditTempBanViewModel
    {
        public Guid AdminActionId { get; set; }
        public Player2 Player { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Expires { get; set; }
    }
}