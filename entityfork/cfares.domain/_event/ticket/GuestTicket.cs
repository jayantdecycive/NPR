
#region Imports

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using cfacore.shared.domain.attributes;

#endregion

namespace cfares.domain._event._ticket
{
    [SyncUrl("/api/GuestTicket")]
    [System.Data.Linq.Mapping.Table]
    public class GuestTicket : Ticket, IGuestTicket
    {
        [Required, Column]
        public virtual int NumberOfGuests { get; set; }
        
        //mdrake: Gabby is suggesting that this isn't how this works
        //carson: She told my to make it like this again. uncommented 7-10-13
        [Required, Column]
        public override int AllocatedCapacity { get {
            return base.AllocatedCapacity + NumberOfGuests;
        } set {} }

		public override bool IsAllocatedCapacityFixed { get { return false;  } }
    }
}
