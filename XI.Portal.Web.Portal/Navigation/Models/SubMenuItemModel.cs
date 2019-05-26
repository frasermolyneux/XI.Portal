namespace XI.Portal.Web.Portal.Navigation.Models
{
    public class SubMenuItemModel
    {
        public SubMenuItemModel(string text, string controller, string actionName)
        {
            Text = text;
            ActionName = actionName;
            Controller = controller;
        }

        public SubMenuItemModel(string text, string controller, string actionName, object routeValues)
        {
            Text = text;
            ActionName = actionName;
            Controller = controller;
            RouteValues = routeValues;
        }

        public string Text { get; set; }
        public string ActionName { get; set; }
        public string Controller { get; set; }

        public object RouteValues { get; set; }
    }
}