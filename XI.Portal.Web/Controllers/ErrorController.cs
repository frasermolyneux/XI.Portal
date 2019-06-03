using System;
using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(
            IContextProvider contextProvider,
            IDatabaseLogger databaseLogger) : base(contextProvider, databaseLogger)
        {
        }

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