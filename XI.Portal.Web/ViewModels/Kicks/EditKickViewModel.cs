using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Kicks
{
    public class EditKickViewModel
    {
        public Player2 Player { get; set; }
        public Guid AdminActionId { get; set; }

        [AllowHtml]
        public string Text { get; set; }
    }
}