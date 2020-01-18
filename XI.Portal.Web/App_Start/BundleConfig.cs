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
                .Include("~/Content/themes/base/core.css")
                .Include("~/Content/themes/base/accordion.css")
                .Include("~/Content/themes/base/autocomplete.css")
                .Include("~/Content/themes/base/button.css")
                .Include("~/Content/themes/base/datepicker.css")
                .Include("~/Content/themes/base/dialog.css")
                .Include("~/Content/themes/base/draggable.css")
                .Include("~/Content/themes/base/menu.css")
                .Include("~/Content/themes/base/progressbar.css")
                .Include("~/Content/themes/base/resizable.css")
                .Include("~/Content/themes/base/selectable.css")
                .Include("~/Content/themes/base/selectmenu.css")
                .Include("~/Content/themes/base/sortable.css")
                .Include("~/Content/themes/base/slider.css")
                .Include("~/Content/themes/base/spinner.css")
                .Include("~/Content/themes/base/tabs.css")
                .Include("~/Content/themes/base/tooltip.css")
                .Include("~/Content/themes/base/theme.css")
            
            );

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

            //free-jqgrid
            bundles.Add(new ScriptBundle("~/bundles/free-jqgrid")
                .Include("~/Scripts/free-jqGrid/jquery.jqgrid.min.js")
            );

            bundles.Add(new StyleBundle("~/content/free-jqgrid")
                .Include("~/Content/ui.jqgrid.min.css")
                .Include("~/Content/custom-jqgrid.css")
            );

            //moment
            bundles.Add(new ScriptBundle("~/bundles/moment")
                .Include("~/Scripts/moment.min.js")
            );

            //datepicker
            bundles.Add(new ScriptBundle("~/bundles/datepicker")
                .Include("~/Scripts/bootstrap-datepicker.min.js")
            );

            bundles.Add(new StyleBundle("~/content/datepicker")
                .Include("~/Content/bootstrap-datepicker.min.css")
            );

            //summernote
            bundles.Add(new ScriptBundle("~/bundles/summernote")
                .Include("~/Scripts/summernote.min.js")
            );

            bundles.Add(new StyleBundle("~/content/summernote")
                .Include("~/Content/summernote.min.css")
            );
        }
    }
}