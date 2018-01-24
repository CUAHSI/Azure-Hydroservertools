using System.Web;
using System.Web.Optimization;

namespace HydroServerTools
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(                      
                        "~/Scripts/jquery-1.10.2.min.js",
                        "~/Scripts/jquery-ui-1.10.3.min.js"
                        ));
                
             
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/bootbox.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/site.css"));

            //Bundles for DataTables 1.10.16 / Editor 1.7.0 et. al.
            bundles.Add(new ScriptBundle("~/bundles/js/DataTablesEditorCombo").Include(
                      "~/Scripts/DT-1.10.16-Btns-1.5.1-Ed-1.7.0-Sel-1.2.4/DT-1.10.16-Btns-1.5.1-Ed-1.7.0-Sel-1.2.4.js",
                      "~/Scripts/DataTables-Editor-1.7.0/editor.bootstrap.js",
                      "~/Scripts/DataTables-KeyTable-2.3.2/js/dataTables.keyTable.js"));

            bundles.Add(new StyleBundle("~/Content/css/DataTablesEditorCombo").Include(
                      "~/Content/DT-1.10.16-Btns-1.5.1-Ed-1.7.0-Sel-1.2.4/DT-1.10.16-Btns-1.5.1-Ed-1.7.0-Sel-1.2.4.css",
                      "~/Content/DataTables-Editor-1.7.0/editor.bootstrap.css",
                      "~/Content/DataTables-KeyTable-2.3.2/css/keyTable.dataTables.css"));

        }
    }
}
