using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using cfacore.shared.domain.attributes;
using cfares.domain._event._ticket;

namespace cfares.domain._event._tickets
{
    [SyncUrl("/api/DateTicket")]
    [ModelId("TicketId","int")]
    public class DateTicket : GuestTicket
    {
        public string TableRequest { get; set; }

        [Required]
        [Column]
		[Range(1, 8, ErrorMessage = "At least one guest is required")]
		public override int NumberOfGuests { get; set; }

        public override int AllocatedCapacity 
        {
            get
            {
				// Gabby: For Influence events reservations count as tickets, guest counts don't matter
				
				// TODO - SH - Very poor modelling present, needs fixing - AllocatedCapacity is obtuse 
				// .. ( should be Tickets int, DateTicket > DateReservation ), NumberOfGuests should be 
				// .. GuestCount, GuestCount should be moved into ITicket, ITicket should be IReservation
				// .. No time given to fix unfortunately

	            return 1;
            }
            set { }
        }

		public override bool IsAllocatedCapacityFixed { get { return true;  } }

        public string SlotStart
        {
            get
            {
                if (Slot == null)
                    return null;
                return Slot.Start.ToString();
            }
            set { }
        }

        public string OwnerZip
        {
            get
            {
                if (Owner == null)
                    return null; 
                return Owner.ZipCode;
            }
            set { }
        }

        public int GroupSize { get { return NumberOfGuests + 1; } }
    }
}
