using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace cfaresv2.Controllers
{
    public class MetaController : Controller
    {
        //
        // GET: /Meta/

        public ActionResult Index()
        {
            return View();
        }

        public RedirectResult WWW(string url) {
            url = Regex.Replace(url,@"^www\.","");
            
            return RedirectPermanent(url);
        }

        public ActionResult HeartBeat()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

    }
}
