using cfacore.shared.domain.attributes;
using cfares.domain._event.menu;
using cfares.domain._event.occ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain._event.slot
{
    [SyncUrl("/api/Slot")]
    public class GiveawaySlot : Slot
    {
        public ICollection<MenuItem> Items { get; set; }
        public virtual GiveawayOccurrence GiveawayOccurrence { get { return (GiveawayOccurrence)Occurrence; } }
        
    }
}
