using System.Web.Mvc;

namespace cfaresv2.Areas.cfatour
{
    public class cfatourAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "cfatour";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "cfatour_default",
                "cfatour/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
