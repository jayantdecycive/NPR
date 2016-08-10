using System;
using System.Collections.Generic;
using System.Linq;
using cfacore.domain._base;
using cfacore.shared.domain._base;
using cfacore.shared.domain.store;
using core.synchronization.Automation;
using cfacore.shared.domain.attributes;
using cfares.domain._event._ticket;
using cfares.domain.store;

namespace cfares.domain._event
{
    [ITable]
    [SyncUrl("/DataService/ResEvent.svc/Occurrences")]
    public interface IOccurrence:IDomainObject 
    {
        TicketCollection Tickets{get;}
        IList<DateTime> GetDates(); 
        string StoreId { get; set; }
        IList<Slot> SlotsList { get; set; }

        ResStore Store { get; set; }

        DateRange RegistrationAvailability { get; set; }

        DateTime Start { get; set; }

        DateTime End { get; set; }

        DateRange SlotRange { get; set; }

        DateTime SlotRangeStart { get; set; }

        DateTime SlotRangeEnd { get; set; }

        ResEvent ResEvent { get; set; }
        int? ResEventId { get; set; }

        OccurrenceStatus Status { get; set; }

		int OccurrenceId { get; set; }
        bool BoundToPrototype { get; set; }
	    bool FullyBooked { get; }

		string StartDateAsString { get; }

        int TicketsAvailable();
        int TicketsAvailable(IDateRange when);
        int TicketsAvailable(DateTime when);

		bool AreTicketsAvailable();
        bool AreTicketsAvailable(IDateRange when);
        bool AreTicketsAvailable(DateTime when);
        
        ISlot CreateSlot();

        void InvalidateCache();
    }
}
