using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.TempBans
{
    public class CreateTempBanViewModel
    {
        public Guid PlayerId { get; set; }
        public Player2 Player { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        [DataType(DataType.Date)]
        public DateTime Expires { get; set; }
    }
}