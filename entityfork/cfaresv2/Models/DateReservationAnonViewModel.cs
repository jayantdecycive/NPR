using System.ComponentModel.DataAnnotations;
using cfacore.shared.modules.com.datatypes;
using cfares.domain._event._tickets;
using cfares.domain.user;
using cfaresv2.Models;

namespace cfaresv2.Areas.FamilyInfluence.Models
{
    public class DateReservationAnonViewModel : TicketAccountModel<DateTicket>, ITicketAccountModel
    {
        public DateReservationAnonViewModel()
        {
            this.Ticket = new DateTicket();
            this.User = new ResUser();
        }



        public bool StandardTable {
            get { return this.Ticket!=null&&this.Ticket.TableRequest == "standard"; }
            set
            {
                if(this.Ticket!=null)
                this.Ticket.TableRequest = value ? "standard" : "any";
            }
        }

    

       
    }
}