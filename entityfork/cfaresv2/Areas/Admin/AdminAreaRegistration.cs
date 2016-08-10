using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace cfaresv2.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
			context.Routes.Add("ImagesRoute", new Route("Admin/Style/media/{*filename}", new cfaresv2.Areas.Admin.Controllers._base.ImageRouteHandler()));
			//context.Routes.Ignore("Admin/Style/media/font/{filename}");

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller="Home",action = "Index", id = UrlParameter.Optional },
                new[] { "cfaresv2.Areas.Admin.Controllers" }

            );
            RegisterBundles();
        }

        private void RegisterBundles()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			//foreach (var bundle in BundleTable.Bundles)
			//	bundle.Transforms.Clear();

			// No use setting here b/c bundle optimization is set from Global.asax.cs
			//BundleTable.EnableOptimizations = false;
        }   
    }
}
