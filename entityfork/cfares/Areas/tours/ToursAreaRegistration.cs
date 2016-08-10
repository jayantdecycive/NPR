using System.Web.Mvc;

namespace cfares.Areas.tours
{
    public class ToursAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "tours";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.IgnoreRoute("Areas/tours/Content/{*pathInfo}");
            context.Routes.IgnoreRoute("Areas/tours/Scripts/{*pathInfo}");

            context.MapRoute(
                "tours_default",
                "tours/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
