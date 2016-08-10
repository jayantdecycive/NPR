using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using cfares.repository.slot;
using cfaresv2.Controllers;
using npr.domain._event.slot;

namespace cfaresv2.Areas.Tour.Controllers
{
    public class ReservationController : ReservationControllerBase
    {
        //
        // GET: /Tours/Reservation/

        public ActionResult SearchByLocation()
        {
            return Redirect("/npr/Home");
        }

        public ActionResult Register()
        {
            return Redirect("/npr/Reservation/Register?" + Request.QueryString);
        }

        

    }
}
