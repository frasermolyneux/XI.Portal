﻿using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.TempBans
{
    public class LiftTempBanViewModel
    {
        public Player2 Player { get; set; }
        public Guid AdminActionId { get; set; }

        [AllowHtml]
        public string Text { get; set; }
    }
}