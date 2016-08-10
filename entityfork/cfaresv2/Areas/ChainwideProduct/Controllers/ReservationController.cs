
#region Imports

using System.Web.Mvc;
using cfares.domain._event._ticket;
using cfares.site.modules.com.reservations.res;
using cfaresv2.Areas.product.Controllers;

#endregion

namespace cfaresv2.Areas.ChainwideProduct.Controllers
{
    public class ReservationController : ReservationControllerBase<ProductGiveawayWizard>
    {
		protected override ProductGiveawayWizard GetReservationWizard(string step, FoodTicket model)
        {
            return new ChainwideProductGiveawayWizard( model, step );
        }       
    }
}
