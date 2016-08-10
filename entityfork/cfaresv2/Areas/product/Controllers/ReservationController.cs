
#region Imports

using cfares.domain._event._ticket;
using cfares.site.modules.com.reservations.res;

#endregion

namespace cfaresv2.Areas.product.Controllers
{
    public class ReservationController : ReservationControllerBase<ProductGiveawayWizard>
    {
        protected override ProductGiveawayWizard GetReservationWizard(string step, FoodTicket model)
        {
            return new ProductGiveawayWizard( model, step );
        }
    }
}
