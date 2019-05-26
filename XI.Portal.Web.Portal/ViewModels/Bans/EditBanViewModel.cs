using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.Portal.ViewModels.Bans
{
    public class EditBanViewModel
    {
        public Player2 Player { get; set; }
        public Guid AdminActionId { get; set; }

        [AllowHtml]
        public string Text { get; set; }
    }
}