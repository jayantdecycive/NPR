
#region Imports

using System.Web.Mvc;

#endregion

namespace cfaresv2.Controllers
{
    public class DynamicReservationController : ReservationControllerBase
    {
		#region Dynamic Routes

		[ChildActionOnly]
        public virtual ActionResult NavBarPrefix() { return View(); }

		[ChildActionOnly]
        public virtual ActionResult NavBarSuffix() { return View(); }

		[ChildActionOnly]
        public virtual ActionResult OccurrenceSelectList( string id )
        {
			//var r = Model.EventSearchResults.Select(x => x.Occurence);
			//InitializeWizard(AppContext.Event, "Reservation.SearchByLocation", ControllerContext.HttpContext.Request, ViewData);
	        return View( (object) id );
        }

        #endregion
    }
}
