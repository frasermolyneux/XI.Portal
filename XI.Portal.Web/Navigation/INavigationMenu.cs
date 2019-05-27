using System.Collections.Generic;
using System.Security.Principal;
using XI.Portal.Web.Navigation.Models;

namespace XI.Portal.Web.Navigation
{
    public interface INavigationMenu
    {
        List<MenuItemModel> GetMenu(IIdentity identity);
    }
}