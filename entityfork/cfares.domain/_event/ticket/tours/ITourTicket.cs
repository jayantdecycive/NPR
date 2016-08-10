using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core.synchronization.Automation;
using cfares.domain.user;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;

namespace cfares.domain._event.slot
{
    [ITable(Name = "Ticket:Tour")]
    [SyncUrl("/DataService/Ticket.svc/Ticket_Tour_Joined", create = "/Service/Ticket.svc/CreateTourTicket")]
    public interface ITourTicket:ITicket
    {
        [Column]
        int GuestCount { get; set; }
        
        [UIHint("Tables/_Tickets")]
        [DataType("RelationalTable/_Users")]
        ResUserCollection Guests { get; set; }
                
    }
}
