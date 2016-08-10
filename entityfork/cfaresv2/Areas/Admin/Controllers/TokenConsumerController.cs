using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    
    public class TokenConsumerController : AdminController
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
