using System.Web.Mvc;

namespace cfaresv2.Areas.SpecialEvent
{
    public class SpecialEventRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SpecialEvent";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SpecialEvent_default",
                "SpecialEvent/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
