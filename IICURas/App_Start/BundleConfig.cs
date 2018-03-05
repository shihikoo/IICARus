using System.Web;
using System.Web.Optimization;

namespace IICURas
{
    public static class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/iicuras").Include(
            "~/Scripts/IICURas*",
            "~/Scripts/googleanalytics*"));

            bundles.Add(new ScriptBundle("~/bundles/contact").Include(
                "~/Scripts/contact.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjquery").Include(
            "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/faqQuery").Include(
"~/Scripts/googlesheetRead.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapselectjquery").Include(
"~/Scripts/bootstrap-select.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                         "~/Scripts/jquery.validate*","~/Scripts/jquery.unobtrusive*"));

            //bundles.Add(new ScriptBundle("~/bundles/chosenjs").Include(
            //            "~/Scripts/chosen.jquery.min.js"));
            
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/site.css",

                 "~/Content/bootstrap.css",
                 "~/Content/bootstrap-theme.css",
                "~/Content/bootstrap-select.css"

               //"~/Content/bootstrap.css.map",
               // "~/Content/bootstrap-theme.css.map",
               // "~/Content/bootstrap-select.css.map",

                //"~/Content/bootstrap-theme.min.css", 
                //"~/Content/bootstrap.min.css",
               // "~/Content/bootstrap-select.min.css"
                 ));

            //bundles.Add(new StyleBundle("~/Content/chosencss").Include(
            //     "~/Content/chosen.css"
            //     ));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));
        }
    }
}