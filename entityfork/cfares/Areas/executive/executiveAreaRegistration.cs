using System.Web.Mvc;

namespace cfares.Areas.executive
{
    public class executiveAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "executive";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "executive_default",
                "executive/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
