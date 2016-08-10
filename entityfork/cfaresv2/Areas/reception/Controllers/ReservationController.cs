using System.Linq;
using System.Web.Mvc;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.repository.slot;
using cfares.repository.store;
using cfares.site.modules.com.reservations.res;
using cfares.site.modules.repository.ticket;
using cfaresv2.Controllers;
using System.Web.Routing;
using cfaresv2.Models;

namespace cfaresv2.Areas.reception.Controllers
{
    public class ReservationController : ReservationWizardController<ReceptionWizard, GuestTicket>
    {
        public ActionResult Index()
        {
            return Redirect("/Receptions");
        }

        //[WizardStep("Reservation.SearchByLocation")]
        public override ActionResult SearchByLocation()
        {
            if(Request.Url.Host=="cfareception.com")
                return Redirect("http://www.cfareception.com/Receptions");
            return Redirect("Receptions");
        }

        [WizardStep("Reservation.ReceptionList")]
        public ActionResult Receptions(int? id)
        {
            if (Request.Url.Host == "cfareception.com")
                return Redirect("http://www.cfareception.com/Receptions");
                
            return View(Model);
        }

        [WizardStep("Reservation.Reservation")]
        public override ActionResult Reservation(int? id)
        {
            var viewModel = new TicketAccountModel<GuestTicket>(Model);
            if (Model.TicketId != 0)
            {
                var storeRepo = new LocationRepository(ResContext);
                viewModel.StoreOptIn = storeRepo.HasEmailSubscription(Model.Slot.Occurrence.Store, Model.Owner);
            }

            return View(viewModel);
        }

        [HttpPost]
        [WizardStep("Reservation.Reservation")]
        public override ActionResult Reservation(int? id, TicketAccountModel<GuestTicket> ViewModel, FormCollection collection)
        {
            var repo = new ResTicketRepository<GuestTicket>(ResContext);
            var slotRepo = new SlotRepository<Slot>(ResContext);
            ViewModel.Ticket.NumberOfGuests = int.Parse(collection["Ticket.NumberOfGuests"]);

            return PostReservationView(id, ViewModel, collection, repo, slotRepo);
        }

        protected override ReceptionWizard GetReservationWizard(string step, GuestTicket model)
        {
            return new ReceptionWizard(model, step);
        }

        protected override GuestTicket GetModel(RequestContext requestContext)
        {
            var id = requestContext.RouteData.Values["id"] as string;
            if (string.IsNullOrEmpty(id))
            {
                return new GuestTicket();
            }
            else
            {
                int ticketId = int.Parse(id);
                return TicketRepository.GetAll(new[] { "Slot.Occurrence", "Owner" }).First(x => x.TicketId == ticketId) as GuestTicket;
            }
        }
    }
}
