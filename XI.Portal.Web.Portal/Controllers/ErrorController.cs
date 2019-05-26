using System;
using System.Web.Mvc;

namespace XI.Portal.Web.Portal.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult BadRequest()
        {
            return View();
        }

        public ActionResult NoAuth()
        {
            return View();
        }

        public ActionResult CauseError()
        {
            throw new Exception("This is an error");
        }
    }
}