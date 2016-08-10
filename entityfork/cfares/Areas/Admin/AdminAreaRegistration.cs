using System.Web.Mvc;

namespace cfares.Areas.Admin
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
            context.Routes.IgnoreRoute("Areas/Admin/Content/{*pathInfo}");
            context.Routes.IgnoreRoute("Areas/Admin/Scripts/{*pathInfo}");

            context.MapRoute(
                "Admin_wizard",
                "Admin/Wizard/{wizard}/{step}/{domainid}",
                new { area = "Admin", controller = "Wizard", action = "Index", step = 0, domainid = UrlParameter.Optional },
                new string[] { "cfares.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { area="Admin",controller = "Home",action = "Index", id = UrlParameter.Optional},
                new string[]{"cfares.Areas.Admin.Controllers"}
            );
            
        }
    }
}
