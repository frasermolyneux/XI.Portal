using System.Collections.Generic;
using System.Security.Principal;
using XI.Portal.Web.Portal.Navigation.Models;

namespace XI.Portal.Web.Portal.Navigation
{
    public interface INavigationMenu
    {
        List<MenuItemModel> GetMenu(IIdentity identity);
    }
}