﻿using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Bans
{
    public class CreateBanViewModel
    {
        public Guid PlayerId { get; set; }

        [AllowHtml]
        public string Text { get; set; }

        public Player2 Player { get; set; }
    }
}