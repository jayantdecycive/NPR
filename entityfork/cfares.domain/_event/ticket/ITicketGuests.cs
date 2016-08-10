using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfares.domain._event.ticket
{
    public interface ITicketGuests
    {
        int TicketGuestsId { get; set; }

        string GuestName { get; set; }

        int TicketId { get; set; }

    }
}
