using System;
using System.Collections.Generic;
using cfacore.domain._base;
using cfares.domain.store;
using cfares.domain.user;
using core.synchronization.Automation;
using cfacore.shared.domain.attributes;

namespace cfares.domain._event
{
    [ITable]
    [SyncUrl("/DataService/Ticket.svc/Tickets")]
    public interface ITicket : IDomainObject
	{
        string CardNumber { get; set; }

        //string this[string key] { get; set; }

        ResStore GetStore();
        ISlot GetSlot();
        IOccurrence GetOccurrence();

        Slot Slot { get; set; }
        int? SlotId { get; set; }
        
        ResUser Owner { get; set; }
        int? OwnerId { get; set; }

        Dictionary<string, string> AdditionalInformation { get; set; }

        int TicketId { get; set; }

        string CreationSrc { get; set; }
        TicketStatus Status { get; set; }
        int AllocatedCapacity { get; }
	    bool IsAllocatedCapacityFixed { get;  }
        
        DateTime CreatedDate { get; set; }
        DateTime? CanceledDate { get; set; }

        ICollection<TicketTransaction> Transactions { get; set; }

        string AdditionalField(string p1, string p2);
    }

    public enum TicketStatus
    {
        Partial,			// Temp / incomplete
        Locked,		// Requires admin review or temporarily on hold / disabled ( not counted towards TOTALS )
        Reserved,	// Reservation made ( counted towards TOTALS )
        Hidden,		// Reservation made, but also private / not-visible ( not counted towards TOTALS )
        Canceled		// Reservation cancelled
    }
}
