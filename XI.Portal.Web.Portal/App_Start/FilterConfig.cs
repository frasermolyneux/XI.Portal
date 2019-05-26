using System.Web.Mvc;
using XI.Portal.Web.Portal.Extensions;

namespace XI.Portal.Web.Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new UserCheckExtension());
        }
    }
}