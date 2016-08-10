
#region Imports

using System.Linq;
using System.Web.Mvc;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain._event._tickets;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.repository.store;
using cfares.site.modules.com.reservations.res;
using cfares.site.modules.repository.ticket;
using cfaresv2.Controllers;
using cfaresv2.Models;

#endregion

namespace cfaresv2.Areas.FamilyInfluence.Controllers
{
    public class ReservationController : ReservationWizardController<FamilyInfluenceReservationWizard, DateTicket>
	{
		// GET: /Reservation
        //Model: Occurrence
        [WizardStep("Reservation.Reservation")]
		public override ActionResult Reservation(int? id)
        {
            var occurenceRepo = new OccurrenceRepository<DateOccurrence>(ResContext);
            int? occurrenceId = Model.Slot == null ? Wizard.OccurrenceId : Model.Slot.OccurrenceId;

            if (occurrenceId == null)
                return PrevStepResult();

		    var occurrence = occurenceRepo.Find(occurrenceId.Value);
		    ViewBag.Occurrence = occurrence;

		    if (!occurrence.AreTicketsAvailable())
		    {
		        return RedirectToAction("SearchByLocation", new {message = "Sorry, that location has filled up"});
		    }
            
            var viewModel = new TicketAccountModel<DateTicket>
	                            {
                    Ticket = Model
                };
            if (Model.TicketId != 0)
            {
                var storeRepo = new LocationRepository(ResContext);
                viewModel.StoreOptIn = storeRepo.HasEmailSubscription(Model.Slot.Occurrence.Store, Model.Owner);
            }
            
            return GetReservationView(viewModel, occurenceRepo);
        }

        [HttpPost]
        [WizardStep("Reservation.Reservation")]
        public override ActionResult Reservation(int? id, TicketAccountModel<DateTicket> ViewModel, FormCollection collection)
        {
            if(id!=null)
                ViewModel.Ticket.TicketId = id.Value;

			var occurenceRepo = new OccurrenceRepository<DateOccurrence>(ResContext);
            int? occurrenceId = Model.Slot == null ? Wizard.OccurrenceId : Model.Slot.OccurrenceId;
            if (occurrenceId == null)
                return PrevStepResult();

		    var occurrence = occurenceRepo.Find(occurrenceId.Value);
		    ViewBag.Occurrence = occurrence;   

            ViewModel.Ticket.TableRequest = bool.Parse(collection["StandardTable"].Split(',')[0])? "standard" : "any";
            var repo = new ResTicketRepository<DateTicket>(ResContext);
            var slotRepo = new SlotRepository<Slot>(ResContext);
            return PostReservationView(id, ViewModel, collection, repo, slotRepo);
        }

        public new ActionResult FAQ()
        {
            return View();
        }

        protected override FamilyInfluenceReservationWizard GetReservationWizard(string step, DateTicket model)
        {
            var _wiz = new FamilyInfluenceReservationWizard(model, step);

            return _wiz;
        }

        protected override DateTicket GetModel(System.Web.Routing.RequestContext requestContext)
        {
            var id = requestContext.RouteData.Values["id"] as string;
            if (string.IsNullOrEmpty(id))
                return new DateTicket();

			int ticketId = int.Parse(id);
	        return TicketRepository.GetAll(new[] { "Slot.Occurrence.Store", "Owner" }).First(x => x.TicketId == ticketId) as DateTicket;
        }
    }
}
