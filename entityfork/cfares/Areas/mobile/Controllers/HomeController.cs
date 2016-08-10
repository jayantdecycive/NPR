using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cfares.Areas.mobile.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /mobile/Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        public ActionResult Directions()
        {
            return View();
        }

    }
}
