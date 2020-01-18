using System.Collections.Generic;

namespace XI.Portal.Web.Navigation.Models
{
    public class MenuItemModel
    {
        public MenuItemModel(string text, string controller, string actionName, string icon)
        {
            Text = text;
            ActionName = actionName.ToLower();
            Controller = controller.ToLower();
            Icon = icon;

            SubMenuItems = new List<SubMenuItemModel>();
        }

        public MenuItemModel(string text, string controller, string actionName, string icon, object routeValues)
        {
            Text = text;
            ActionName = actionName.ToLower();
            Controller = controller.ToLower();
            Icon = icon;
            RouteValues = routeValues;

            SubMenuItems = new List<SubMenuItemModel>();
        }

        public string Text { get; set; }
        public string ActionName { get; set; }
        public string Controller { get; set; }
        public string Icon { get; set; }
        public object RouteValues { get; set; }

        public List<SubMenuItemModel> SubMenuItems { get; set; }
    }
}