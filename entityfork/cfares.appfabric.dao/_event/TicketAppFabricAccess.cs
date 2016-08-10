using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.dao._base;
using cfares.domain._event;
using cfares.domain._event._ticket.tours;

namespace cfares.appfabric.dao._event
{
    public class TicketAppFabricAccess : AppFabricAccess<Ticket>
    {
        public TicketAppFabricAccess(string connectionString)
            : base(connectionString)
        {
            this.ConnectionString = connectionString;
        }
                
        public TourTicket LoadTour(string cid){
            TourTicket tempTicket = new TourTicket();
            tempTicket.Id(cid);
            return (TourTicket)Cache.Get(tempTicket.Uri().ToString());
        }

    }
}
