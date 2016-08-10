using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfares.domain._event;
using cfares.domain._event.slot;
using cfares.domain._event._ticket.tours;
using cfares.repository.ticket;
using cfares.site.modules.com.reservations.cfatours;
using cfares.site.modules.repository.ticket;
using cfaresv2.Controllers;
using System.Web.Routing;
using cfaresv2.Models;

namespace cfaresv2.Areas.cfatours.Controllers
{
    public class ReservationController : ReservationWizardController<TourReservationWizard,TourTicket>
    {
        //
        // GET: /cfatours/Reservation/

        private ResTicketRepository<TourTicket> ticketRepository;

        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ticketRepository = new ResTicketRepository<TourTicket>(UserMembershipRepo.Context);
        }

        protected override TourTicket GetModel(RequestContext filterContext)
        {
            int? id = ToNullableInt32(filterContext.RouteData.Values["id"] as string);
            if (id == null)
            {
                return new TourTicket();
            }
            else
            {
                return ticketRepository.Find(x=>x.TicketId==id) as TourTicket;
            }
        }

        protected override TourReservationWizard GetReservationWizard(string step, TourTicket model)
        {
            return new TourReservationWizard(model, step);
        }


        public ActionResult Index()
        {
            return Content("Hello World");
        }

        [WizardStep("Reservaiton.Reservation")]
        public override ActionResult Reservation(int? id)
        {
            throw new NotImplementedException();
        }

        public override ActionResult Reservation(int? id, TicketAccountModel<TourTicket> ViewModel, FormCollection collection)
        {
            throw new NotImplementedException();
        }
    }
}
