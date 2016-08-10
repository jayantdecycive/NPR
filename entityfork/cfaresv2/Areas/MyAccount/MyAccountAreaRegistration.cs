using System.Web.Mvc;

namespace cfaresv2.Areas.MyAccount
{
    public class MyAccountAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MyAccount";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MyAccount",
                "Account/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional },
                new[] { "cfaresv2.Areas.MyAccount.Controllers" }
            );
        }
    }
}
