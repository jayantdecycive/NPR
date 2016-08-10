
#region Imports

using System;
using System.Linq;
using System.Web.Mvc;
using cfacore.shared.domain._base;
using cfares.domain._event;
using cfares.domain._event.occ;
using cfares.domain._event._ticket;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.site.modules.com.reservations.res;
using cfares.site.modules.repository.ticket;
using cfaresv2.Controllers;
using System.Web.Routing;
using cfaresv2.Models;
using DevTrends.MvcDonutCaching;
#endregion

namespace cfaresv2.Areas.product.Controllers
{
    public abstract class ReservationControllerBase<TWizard> : ReservationWizardController<TWizard, FoodTicket>         
        where TWizard : ProductGiveawayWizard, IReservationWizard<FoodTicket>
    {
		[DynamicOutputCache]
		public ActionResult Index()
        {
            return View();
        }

		[DynamicOutputCache]
        public ActionResult Rules()
        {
            return View();
        }

        [WizardStep("Reservation.Calendar")]
		[DynamicOutputCache]
        public ActionResult Calendar(int? id)
        {
			//CacheContext.UseCacheProfile( CacheProfile.StaticPage );

            string occurrenceIdStr;


            if (Model.SlotId != null && Model.Slot.OccurrenceId != null)
            {
                occurrenceIdStr = Model.Slot.OccurrenceId.ToString();
                if (!String.IsNullOrEmpty(Request.QueryString["occurrence"]) && occurrenceIdStr != Request.QueryString["occurrence"])
                    occurrenceIdStr = Request.QueryString["occurrence"];
            }
            else
                occurrenceIdStr = Request.QueryString["occurrence"];

            if (string.IsNullOrEmpty(occurrenceIdStr))
                return PrevStepResult();

            var occurrenceRepo = new OccurrenceRepository<GiveawayOccurrence>(TicketRepository.Context);
            var occurrenceId = int.Parse(occurrenceIdStr);
            GiveawayOccurrence occurrence = occurrenceRepo.Find(x=>x.OccurrenceId==occurrenceId,"Store") as GiveawayOccurrence;
            Wizard.Prime(occurrence);
            ViewBag.Occurrence = occurrence;
            return View(Model);
        }

        [WizardStep("Reservation.Reservation")]
		[DynamicOutputCache]
        public override ActionResult Reservation(int? id)
        {
            if (Wizard.SlotId!=null)
            Model.SlotId = Wizard.SlotId;
            if (Wizard.FoodId != null)
            Model.MenuItemId = Wizard.FoodId;
            
            var occurrenceRepo = new OccurrenceRepository<GiveawayOccurrence>(TicketRepository.Context);
            var viewModel = new TicketAccountModel<FoodTicket>(Model);
            Wizard.Prime(Model);
            return GetReservationView(viewModel, occurrenceRepo);
        }

        [HttpPost]
        [WizardStep("Reservation.Reservation")]
		[DynamicOutputCache]
        public override ActionResult Reservation(int? id, TicketAccountModel<FoodTicket> _ViewModel,FormCollection collection)
        {
            var ViewModel = _ViewModel;
            ViewModel.Ticket.AdditionalField("TriedBreakfast", collection["TriedBreakfast"]);
            try
            {
                ViewModel.Ticket.SlotId = int.Parse(collection["Ticket.SlotId"]);
                ViewModel.Ticket.MenuItemId = (collection["Ticket.MenuItemId"]);
            }
            catch
            {
                return View(ViewModel);
            }
            var repo = new ResTicketRepository<FoodTicket>(ResContext);
            var slotRepo = new SlotRepository<Slot>(ResContext);
            return PostReservationView(id, ViewModel,collection,repo,slotRepo);
        }

        [WizardStep("Reservation.Food")]
		[DynamicOutputCache]
        public ActionResult Food(int? id)
        {
            string occurrenceIdStr;

            if (Model.SlotId != null && Model.Slot.OccurrenceId != null)
                occurrenceIdStr = Model.Slot.OccurrenceId.ToString();
            else
                occurrenceIdStr = Request.QueryString["occurrence"];

            if (string.IsNullOrEmpty(occurrenceIdStr))
                return PrevStepResult();

            if (Wizard.SlotId != null)
                Model.SlotId = Wizard.SlotId;
            if (Wizard.FoodId != null)
                Model.MenuItemId = Wizard.FoodId;

            var occurrenceRepo = new OccurrenceRepository<GiveawayOccurrence>(TicketRepository.Context);
            var occurrenceId = int.Parse(occurrenceIdStr);
            GiveawayOccurrence occurrence = occurrenceRepo.Find(x => x.OccurrenceId == occurrenceId, "Store") as GiveawayOccurrence;
            Wizard.Prime(Model);
            ViewBag.Occurrence = occurrence;
            return View(Model);
        }

        protected override FoodTicket GetModel(RequestContext requestContext)
        {
            var id = requestContext.RouteData.Values["id"] as string;
            if (string.IsNullOrEmpty(id))
                return new FoodTicket();
	        
			int ticketId = int.Parse(id);
	        return (FoodTicket)TicketRepository.GetAll(new[] { "Slot.Occurrence", "Item","Owner" }).First(x => x.TicketId == ticketId);
        }
    }
}
