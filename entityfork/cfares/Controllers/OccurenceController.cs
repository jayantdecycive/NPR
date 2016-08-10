using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.domain._event;

namespace cfares.Controllers
{
    public class OccurenceController : Controller
    {
        private IOccurrenceCollection occurences;

        public OccurenceController(IOccurrenceCollection collectionArg)
        {
            occurences = collectionArg;
        }

        public ViewResult viewListOfRegisteredOccurences()
        {
            //Build EventFilter object here

            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
