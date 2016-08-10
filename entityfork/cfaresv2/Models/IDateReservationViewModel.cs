using cfares.domain._event._ticket;
using cfares.domain._event._tickets;
using cfares.domain.user;

namespace cfaresv2.Areas.FamilyInfluence.Models
{
    public interface IDateReservationViewModel
    {
        //DateTicket Ticket { get; set; }
        ResUser User { get; set; }
    }
}