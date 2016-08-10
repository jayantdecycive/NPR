using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using cfares.domain._event.menu;
using cfares.domain._event.occ;

namespace cfares.domain._event.resevent.store
{
    public class GiveawayEvent:StoreEvent,IGiveawayEvent
    {
        public virtual ICollection<MenuItemAllowance> ProductAllowances { get; set; }
        public virtual IEnumerable<MenuItem> AllowedProducts
        {
            get
            {
                if (ProductAllowances==null)
                    return new Collection<MenuItem>();
                return ProductAllowances.Select(x => x.AllowedItem);
            }
        }
        public int MaxItems { get; set; }
        public int MinItems { get; set; }
        //public new ICollection<GiveawayOccurrence> Occurrences { get; set; }
    }
}
