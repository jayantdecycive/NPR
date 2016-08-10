using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cfares.Areas.Tours.Controllers
{
    [UserModeFilter(userMode = "Customer")]
    public class HomeController : Controller
    {

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult CustomerPlan()
        {
            return View();
            
        }

        public ActionResult Index()
        {
            return View();

        }
    }
}
