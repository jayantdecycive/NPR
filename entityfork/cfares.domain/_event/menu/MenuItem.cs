using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using cfacore.shared.domain.media;
using cfares.domain._event.resevent.store;

namespace cfares.domain._event.menu
{
    public class MenuItem
    {
        
        public string DomId { get; set; }
        public string Name { get; set; }
        
        public string ShortName { get; set; }
        public virtual Media Media { get; set; }
        public int? MediaId { get; set; }

        public virtual ICollection<MenuItemAllowance> EventAllowances { get; set; }

        public virtual IEnumerable<GiveawayEvent> AppliedEvents
        {
            get
            {
                if (EventAllowances == null)
                    return new Collection<GiveawayEvent>(); 
                return EventAllowances.Select(x => x.ResEvent);
            }
        }
    }
}
