using System.Web.Mvc;
using cfaresv2.Controllers;

namespace cfaresv2.Areas.SpecialEvent.Controllers
{
    public class ReservationController : ReservationControllerBase
    {
        //
        // GET: /SpecialEvent/Reservation/

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
