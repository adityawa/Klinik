using System.Web;
using System.Web.Optimization;

namespace Klinik.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/cssklinik").Include(
                     "~/Content/plugins/colorpicker/colorpicker.css",
                     "~/Content/custom-plugins/picklist/picklist.css",
                     "~/Content/bootstrap/css/bootstrap.min.css",
                     "~/Content/css/fonts/ptsans/stylesheet.css",
                     "~/Content/css/fonts/icomoon/style.css",
                     "~/Content/css/mws-style.min.css",
                     "~/Content/css/icons/icol16.css",
                     "~/Content/css/icons/icol32.css",
                     "~/Content/plugins/select2/select2.css"
                     ));

            bundles.Add(new StyleBundle("~/Content/jui").Include(
                   "~/Content/jui/css/jquery.ui.all.css",
                   "~/Content/jui/jquery-ui.custom.css",
                   "~/Content/jui/css/jquery.ui.timepicker.css",
                   "~/Content/js/chosen/chosen.css"
                   ));
            bundles.Add(new StyleBundle("~/Content/theme").Include(
                   "~/Content/css/mws-theme.css",
                   "~/Content/css/themer.css",
                   "~/Content/plugins/jgrowl/jquery.jgrowl.css",
                   "~/Content/css/my-style.css"
                   ));

            bundles.Add(new ScriptBundle("~/bundles/jsklinik").Include(
                      "~/Content/js/libs/jquery-1.8.3.min.js",
                      "~/Content/js/libs/jquery.mousewheel.min.js",
                      "~/Content/js/libs/jquery.price.js",
                      "~/Content/js/libs/jquery.placeholder.min.js",
                      "~/Content/custom-plugins/fileinput.min.js",
                      "~/Content/jui/js/jquery-ui-1.9.2.min.js",
                      "~/Content/jui/jquery-ui.custom.min.js",
                      "~/Content/jui/js/jquery.ui.touch-punch.min.js",
                      "~/Content/jui/js/timepicker/jquery-ui-timepicker.min.js",
                      "~/Content/plugins/select2/select2.min.js",
                       "~/Content/custom-plugins/picklist/picklist.min.js",
                       "~/Content/plugins/datatables/jquery.dataTables.min.js",
                       "~/Content/plugins/colorpicker/colorpicker-min.js",
                       "~/Content/plugins/validate/jquery.validate-min.js",
                         "~/Content/plugins/flot/jquery.flot.min.js",
                       "~/Content/plugins/flot/plugins/jquery.flot.tooltip.min.js",
                       "~/Content/plugins/flot/plugins/jquery.flot.pie.min.js",
                       "~/Content/plugins/flot/plugins/jquery.flot.resize.min.js",
                        "~/Content/plugins/flot/plugins/jquery.flot.categories.js",
                        "~/Content/plugins/flot/plugins/jquery.flot.stack.min.js",
                        "~/Content/plugins/special/jquery.validate.js",
                        "~/Content/plugins/special/jquery.mousewheel.js",
                        "~/Content/plugins/special/jquery.deserialize.js",
                        "~/Content/plugins/special/jquery.hotkeys.js",
                        "~/Content/plugins/special/jquery.scrollto.js",
                        "~/Content/plugins/special/jquery.masked.js",
                        "~/Content/plugins/jgrowl/jquery.jgrowl-min.js",
                        "~/Content/js/chosen/chosen.jquery.min.js",
                        "~/Content/bootstrap/js/bootstrap.min.js",
                        "~/Content/js/core/mws.js",
                        "~/Content/js/core/themer.js",
                        "~/Content/js/buzz.js",
                        "~/Content/js/core/antrian.js",
                        "~/Content/js/highcharts/highcharts.js",
                        "~/Content/js/highcharts/modules/exporting.js"
                      ));
        }
    }
}
