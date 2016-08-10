
#region Imports

using System.Data.Entity;
using cfares.domain._event.menu;
using cfares.domain._event.resevent.store;
using cfares.domain._event.slot;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace cfares.domain._event.occ
{
    public class GiveawayOccurrence : Occurrence
    {
	    public virtual IEnumerable<GiveawaySlot> GiveawaySlotsList {
            get { return SlotsList.Where(x=>x is GiveawaySlot).Cast<GiveawaySlot>(); }            
        }

	    private IList<MenuItem> _itemsAvailable;
        public virtual IList<MenuItem> ItemsAvailable
        {
	        get
	        {
                if (_itemsAvailable != null && _itemsAvailable.Count == 0)
                {
					// Return list of all defaulted on and required on products for this Occurrence
					foreach (MenuItemAllowance a in ((GiveawayEvent)ResEvent).ProductAllowances
						.AsQueryable().Include("AllowedItem.Media").ToList())
					{
						if( a.Condition == MenuItemCondition.LockedOn ||
							a.Condition == MenuItemCondition.DefaultOn )
							_itemsAvailable.Add( a.AllowedItem );
					}
				}
				return _itemsAvailable;
	        }
	        set { _itemsAvailable = value; }
        }

        public virtual IList<MenuItem> ItemsAvailableWithoutDefaults
        {
	        get { return _itemsAvailable ?? new List<MenuItem>(); }
	        set { _itemsAvailable = value; }
        }
    }
}
