using System.Web;
using System.Web.Optimization;

namespace Blogging
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // CSS Files
            bundles.Add(new StyleBundle("~/css/main").Include(
                      "~/Content/adminlte.css",
                      "~/Content/OverlayScrollbars.css",
                      "~/Content/style.css"));

            bundles.Add(new StyleBundle("~/css/daterangepicker").Include(
                      "~/Plugins/daterangepicker/daterangepicker.css"));
            
            bundles.Add(new StyleBundle("~/css/icheck").Include(
                      "~/Plugins/icheck-bootstrap/icheck-bootstrap.css"));

            bundles.Add(new StyleBundle("~/css/select2").Include(
                      "~/Plugins/select2/select2.css"));

            // JavaScript Files
            bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            
            bundles.Add(new ScriptBundle("~/js/jquery-ui").Include(
                        "~/Scripts/jquery-ui.js"));

            bundles.Add(new ScriptBundle("~/js/main").Include(
                      "~/Scripts/jquery-3.4.1.min.js",
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/adminlte.js",
                      "~/Scripts/overlayScrollbars.js"));
            
            bundles.Add(new ScriptBundle("~/js/select2").Include(
                      "~/Plugins/select2/select2.js"));
            
            bundles.Add(new ScriptBundle("~/js/sweetalert2").Include(
                      "~/Plugins/sweetalert2/sweetalert2.js"));
            
            bundles.Add(new ScriptBundle("~/js/daterangepicker").Include(
                      "~/Plugins/daterangepicker/moment.js",
                      "~/Plugins/daterangepicker/daterangepicker.js"));
            
            bundles.Add(new ScriptBundle("~/js/moment").Include(
                      "~/Plugins/daterangepicker/moment.js"));

        }
    }
}
