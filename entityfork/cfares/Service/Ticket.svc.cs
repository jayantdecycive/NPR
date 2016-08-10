using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using cfares.domain.user;
using cfacore.site.controllers._event;
using cfares.domain._event.ticket;
using cfares.DataService;

namespace cfares.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Ticket" in code, svc and config file together.
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Ticket : ITicket
    {
        TicketService serv = new TicketService();
        public void DoWork()
        {
        }


        public domain._event.slot.ITourTicket CreateTourTicket(int CardNumber, string SlotId, string OwnerId,string TicketId, int GuestCount)
        {
            if (Validation.GetFormsTicket()==null)
                throw new System.Security.Authentication.AuthenticationException("Access Denied");

            cfares.domain._event.ticket.tours.TourTicket newTicket = new cfares.domain._event.ticket.tours.TourTicket();

            newTicket.CardNumber = CardNumber;
            newTicket.Slot = new cfares.domain._event.slot.tours.TourSlot(SlotId);
            newTicket.Owner = new ResUser(OwnerId);
            newTicket.GuestCount = GuestCount;

            /*
             * Important! This effectively means that, if a media id is passed, the existing user will be upgraded to be a TourSlot
             * */
            if (!String.IsNullOrWhiteSpace(TicketId))
                newTicket.Id(TicketId);

            serv.Save(newTicket);

            return newTicket;
        }
    }
}
