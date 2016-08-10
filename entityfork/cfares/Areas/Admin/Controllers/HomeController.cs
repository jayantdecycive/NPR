using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.Global;

namespace cfares.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,cfa")]
    public class HomeController : Controller
    {

        ReservationContextHint applicationInfo = null;
        

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {            
            applicationInfo = (ReservationContextHint)HttpContext.Items["reservation-context"];
        }

        //
        // GET: /Admin/Home/

        public ActionResult Index()
        {
            
            switch(applicationInfo.Mode){
                case "res":
                    return View("Res-Landing");
                case "tours":
                default:
                    return View("Tours-Landing");
            }

            
        }

    }
}
