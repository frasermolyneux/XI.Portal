using System.Web.Optimization;

namespace XI.Portal.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            //jquery
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-3.4.1.min.js")
                .Include("~/Scripts/jquery-validate.min.js")
                .Include("~/Scripts/jquery-ui-1.12.1.min.js")
            );

            bundles.Add(new StyleBundle("~/content/jquery")
                .Include("~/Content/themes/base/all.css"));

            //popper
            bundles.Add(new ScriptBundle("~/bundles/popper")
                .Include("~/Scripts/umd/popper.min.js")
            );

            //bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap")
                .Include("~/Scripts/bootstrap.min.js")
            );

            bundles.Add(new StyleBundle("~/content/bootstrap")
                .Include("~/Content/bootstrap.min.css")
            );

            //font-awesome
            bundles.Add(new StyleBundle("~/content/font-awesome")
                .Include("~/Content/font-awesome.min.css")
            );

            //animate
            bundles.Add(new StyleBundle("~/content/animate")
                .Include("~/Content/animate.min.css")
            );

            //inspinia theme
            bundles.Add(new StyleBundle("~/content/inspinia")
                .Include("~/Content/inspinia-theme-style.css")
            );
        }
    }
}