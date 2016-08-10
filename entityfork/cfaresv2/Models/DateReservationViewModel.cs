using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cfares.domain._event;
using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;

namespace cfaresv2.Areas.FamilyInfluence.Models
{
    public class DateReservationViewModel : IDateReservationViewModel
    {
        public virtual DateTicket Ticket { get; set; }
        public virtual ResUser User
        {
            get
            {
                if(Ticket==null)
                    Ticket = new DateTicket(){Owner = new ResUser()};
                
                return Ticket.Owner;
            }
            set
            {
                if (Ticket == null)
                    Ticket = new DateTicket() { Owner = new ResUser() }; 
                Ticket.Owner = value;
            }
        }

        
    }
}