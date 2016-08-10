using System.Web.Mvc;

namespace cfaresv2.Areas.npr
{
    public class nprAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "npr";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
               "npr_default",
               "npr/{controller}/{action}/{id}",
               new { action = "Index", id = UrlParameter.Optional }
           );

            context.MapRoute(
               "npr",
               "{permalink}",
               new { controller = "Home", action = "GetBySlug" }//,
                //  new { permalink = new CmsUrlConstraint() },
                // new[] { "cfaresv2.Areas.npr.Controllers" }
           );
            
        }
    }
}
