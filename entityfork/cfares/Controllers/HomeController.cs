using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfacore.site.controllers.shared;
using cfares.domain._event;
using cfacore.shared.domain.user;
using cfacore.service;
using cfacore.shared.modules.user;
using cfares.domain.user;

namespace cfares.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

            UserMembershipService memServ = new UserMembershipService(HttpContext);

            ResUser currentUser = memServ.GetUserAndAccount().ApplicationUser;

            Console.Write(currentUser.Name);

            return View();
        }

        public ActionResult About()
        {

            UserMembershipService memServ = new UserMembershipService(HttpContext);

            ResUser currentUser = memServ.GetUserAndAccount().ApplicationUser;

            Console.Write(currentUser.Name);
            
            return View();
        }
    }
}
