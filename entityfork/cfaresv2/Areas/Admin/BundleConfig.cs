using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace cfaresv2.Areas.Admin
{
    public static class BundleConfig
    {
        

        public static string[] backbone = new string[]{
          "~/Scripts/backbone/models.js",
          "~/Scripts/backbone/managers.js",
          };
        public static string[] scripts = new string[]{"~/Scripts/jquery-1.8.3.js",
          
          "~/Scripts/modernizr-1.7.js",
          
             "~/Scripts/jquery/plugins.js",
            //"~/Scripts/jquery-ui-1.8.11.js",
			//"~/Scripts/jquery-ui-1.9.2.custom.js",
			"~/Scripts/jquery-ui-1.10.2.custom.js",
			"~/Scripts/datajs-1.0.3.js",
            
            "~/Scripts/jquery.validate.js",
            "~/Scripts/jquery.validate.unobtrusive.min.js",
          "~/Scripts/api/api-base.js",
            
          "~/Scripts/Markdown.Converter.js",
          
          
          //"~/Scripts/datajs-1.0.3.min.js",
		  //"~/Scripts/jaydata.js",
		  //"~/Scripts/jaydataproviders/oDataProvider.js",
		  //"~/Scripts/ResEntities.js",
        "~/Areas/Admin/Scripts/Admin.js",
            "~/Scripts/Underscore.js",
          //"~/Scripts/date.js",
          "~/Scripts/backbone.dev.js",
          "~/Scripts/backbone/Url.js",       

          "~/Scripts/backbone/pagination.js",
          "~/Scripts/backbone/queryable.js",
          "~/Scripts/backbone/order.js",
          "~/Scripts/backbone/backbone-relational.js",
          "~/Scripts/knockout-2.1.0.js",
          "~/Scripts/knockback.dev.js",
          "~/Scripts/backbone.localStorage.js",
          
          
                "~/Scripts/jquery.dataTables.js",      
            "~/Scripts/jquery/jquery.jeditable.js",
                "~/Scripts/datatables/KeyTable.js",
          "~/Scripts/datatables/jquery.dataTables.editable.js",     
          "~/Scripts/datatables/plugins.js",

          
          "~/Scripts/datatables/ColVis-1.0.7/media/js/ColVis.js",
          "~/Scripts/datatables/TableTools-2.0.3/media/js/TableTools.js",
          "~/Scripts/datatables/TableTools-2.0.3/media/js/ZeroClipboard.js",
        "~/Scripts/plugins.js",
        "~/Scripts/ajaxupload-min.js",
        "~/Scripts/jcrop/js/jquery.color.js",
        "~/Scripts/jcrop/js/jquery.Jcrop.js",
          
            };

        public static void RegisterBundles(BundleCollection bundles)
        {
            Bundle sb = new Bundle("~/Admin/Script/shared.js");
            Bundle bbb = new Bundle("~/Admin/Script/backbone.js");            
            sb.Include(scripts);
            
            bbb.Include(backbone);

			// No use setting here b/c bundle optimization is set from Global.asax.cs
			//BundleTable.EnableOptimizations = false;
                 
            bundles.Add(sb);
            bundles.Add(bbb);

            bundles.Add(new StyleBundle("~/Admin/Style/shared.css").Include(
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
        "~/Content/themes/base/jquery.ui.theme.css",

        "~/Content/Reset.css",
        "~/Content/Site.css",
        "~/Areas/Admin/Content/Site.css",
        "~/Content/plugins.css",
        "~/Content/font-awesome.css",
        "~/Content/themes/admin-blue/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-orange/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-orange-invert/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-red/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-green/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-table/jquery-ui-1.8.18.custom.css",
        "~/Content/themes/admin-grey-2/jquery-ui-1.10.2.custom.css",
		"~/Content/themes/admin-table-2/jquery-ui-1.10.2.custom.css",
        "~/Scripts/datatables/ColVis-1.0.7/media/css/ColVis.css",
        "~/Scripts/datatables/TableTools-2.0.3/media/css/TableTools_JUI.css",
        "~/Scripts/jcrop/css/jquery.Jcrop.css"));
        }
    }
}