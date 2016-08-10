using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Global;

namespace cfares.Areas.tours.Controllers
{
    
    public class HomeController : Controller
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ReservationContextHint applicationInfo = (ReservationContextHint)HttpContext.Items["reservation-context"];

            if (applicationInfo.EventId == "team")
            {
                ViewBag.UserMode = "Operator";
            }
            else
            {
                ViewBag.UserMode = "Customer";
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult CustomerPlan()
        {
            return View("Directions");
        }

        public ActionResult Index()
        {
            if(Request.Browser.IsMobileDevice){
                return Redirect("http://m.story.tours.chick-fil-a.com");
            }
            return View();
        }
    }
}
