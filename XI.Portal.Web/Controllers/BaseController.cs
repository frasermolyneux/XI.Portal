using System.Web.Mvc;
using XI.Portal.Data.Core.Context;
using XI.Portal.Library.Logging;

namespace XI.Portal.Web.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(IContextProvider contextProvider, IDatabaseLogger databaseLogger)
        {
            ContextProvider = contextProvider;
            DatabaseLogger = databaseLogger;
        }

        protected IContextProvider ContextProvider { get; }
        protected IDatabaseLogger DatabaseLogger { get; }
    }
}