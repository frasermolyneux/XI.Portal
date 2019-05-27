using System.Collections.Generic;
using XI.Portal.Data.Core.Models;

namespace XI.Portal.Web.ViewModels.Demos
{
    public class DemoIndexViewModel
    {
        public string DemoManagerAuthKey { get; set; }
        public List<Demo> Demos { get; set; }
    }
}