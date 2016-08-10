using System.Web.Mvc;
using System.Web.Security;

namespace cfaresv2.Areas.MyAccount.Controllers
{
    public class HomeController : Controller
    {
        // GET: /MyAccount/Home/
        public ActionResult Index()
        {
            return RedirectToAction( "Index", "Dashboard" );
        }

		public ActionResult LogOff()
        {
			FormsAuthentication.SignOut();
		    if (!string.IsNullOrEmpty(Request.QueryString["RedirectUrl"]))
                return Redirect(Request.QueryString["RedirectUrl"]);
			return UrlHelpers.RedirectToEventHome();
        }
    }
}
