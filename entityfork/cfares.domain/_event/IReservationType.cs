using System.Collections.Generic;
using System.Linq;
using cfacore.domain._base;
using cfares.domain._event.resevent;
using core.synchronization.Automation;
using cfares.domain._event._ticket;
using cfares.domain._event.occ;
using Ninject;

namespace cfares.domain._event
{
    [ITable]    
    public interface IReservationType : IDomainObject
    {
        IQueryable<ResEvent> ActiveEvents { get;  }
        List<ResEvent> EventList { get; set; }
        TicketCollection Tickets { get; } // SH - rlly TicketCache
        OccurrenceCollection Occurrences { get; } // SH - rlly OccurrencesCache
        IKernel GetKernel();
        string Description { get; set; }
                
        string Name { get; set; }

        bool IsEventActive { get; }
        ICollection<ResSiteUrl> SiteUrls { get; set; }

        string ReservationTypeId { get; set; }
    }
}
