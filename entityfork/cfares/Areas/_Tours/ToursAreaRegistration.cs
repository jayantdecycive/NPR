using System.Web.Mvc;

namespace cfares.Areas.Tours
{
    public class ToursAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tours";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.IgnoreRoute("Areas/Tours/Content/{*pathInfo}");
            context.Routes.IgnoreRoute("Areas/Temp/Scripts/{*pathInfo}");

            context.MapRoute(
                "Tours_default",
                "Tours/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
