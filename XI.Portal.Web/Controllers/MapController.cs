﻿using System.Web.Mvc;
using XI.Portal.Library.Auth.XtremeIdiots;

namespace XI.Portal.Web.Controllers
{
    [Authorize(Roles = XtremeIdiotsRoles.LoggedInUser)]
    public class MapController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}