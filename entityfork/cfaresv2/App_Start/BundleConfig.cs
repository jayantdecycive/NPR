using System.Diagnostics;
using System.Web;
using System.Web.Optimization;
using cfares.site.modules.com.application;

namespace cfaresv2
{
    public class BundleConfig
    {
		// SH - Commenting out due to JS client-side validation issues in production
		// [Conditional("DEBUG")] 
		public static void ConfigureOptimizations()
		{
			BundleTable.EnableOptimizations = ReservationConfig.GetConfig().EnableBundling;
		}

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js")); /*,

			// RE: http://connect.microsoft.com/VisualStudio/feedback/details/776965/please-support-jquery-v1-9-0-properly-in-jquery-validate-unobtrusive
			// RE: https://github.com/jquery/jquery-migrate#readme
			// Client side validation fix ( breaking compatibility changes: .live() -> .on(), etc. )
			// .. needed for unobtrusive client-side validation
						"~/Scripts/jquery-migrate-1.1.1.js"));*/

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
						"~/Scripts/bootstrap.js",
						"~/Scripts/bootstrap/bootstrap-select.js"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap-wysiwyg").Include(
						"~/Scripts/bootstrap/bootstrap-wysiwyg.js",
                       // "~/Scripts/ckeditor.js",
						"~/Scripts/jquery/jquery.hotkeys.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
						"~/Content/bootstrap.css",
						"~/Content/plugins/bootstrap/bootstrap-select.css",
						"~/Content/bootstrap-responsive.css"));

            bundles.Add(new ScriptBundle("~/bundles/backbone").Include(
                       
          "~/Scripts/backbone.dev.js",
          "~/Scripts/backbone/Url.js",

          "~/Scripts/backbone/pagination.js",
          "~/Scripts/backbone/queryable.js",
          "~/Scripts/backbone/order.js",
          "~/Scripts/backbone/backbone-relational.js",
          "~/Scripts/knockout-2.1.0.js",
          "~/Scripts/knockback.dev.js",
          "~/Scripts/backbone.localStorage.js"));

            bundles.Add(new ScriptBundle("~/bundles/res").Include(
                        "~/Scripts/api/api-base.js", 
						"~/Scripts/backbone/models.js",
						"~/Scripts/backbone/managers.js", 
						"~/Scripts/hammer.js", 
						"~/Scripts/client-init.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/MyAccount/css").Include("~/Content/Themes/MyAccount/Screen.css"));

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