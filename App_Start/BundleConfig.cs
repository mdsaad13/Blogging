using System.Web;
using System.Web.Optimization;

namespace Blogging
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css/main").Include(
                      "~/Content/adminlte.css",
                      "~/Content/OverlayScrollbars.css",
                      "~/Content/style.css,"));

            bundles.Add(new ScriptBundle("~/js/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/js/main").Include(
                      "~/Scripts/jquery-3.4.1.min.js",
                      "~/Scripts/bootstrap.bundle.min.js",
                      "~/Scripts/adminlte.js",
                      "~/Scripts/overlayScrollbars.js"));
        }
    }
}
