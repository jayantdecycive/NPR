using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cfares.Areas.Admin.Controllers
{
    
    public class TokenConsumerController : Controller
    {
        //
        // GET: /Admin/TokenConsumer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SAML(string id) {
            return Content("Hello world");
        }

    }
}
