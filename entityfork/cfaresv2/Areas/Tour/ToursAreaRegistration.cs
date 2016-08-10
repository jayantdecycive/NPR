using System.Web.Mvc;

namespace cfaresv2.Areas.Tour
{
    public class TourAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Tour";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Tour_default",
                "Tour/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
