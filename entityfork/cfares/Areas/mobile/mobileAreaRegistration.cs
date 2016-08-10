using System.Web.Mvc;

namespace cfares.Areas.mobile
{
    public class mobileAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "mobile";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "mobile_default",
                "mobile/{controller}/{action}/{id}",
                new {controller="home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
