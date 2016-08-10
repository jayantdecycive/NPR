using cfacore.shared.domain.attributes;
using cfares.domain._event.menu;

namespace cfares.domain._event._ticket
{
    [SyncUrl("/api/FoodTicket")]
    [ModelId("TicketId","int")]
    public class FoodTicket : Ticket
    {
	    public virtual MenuItem Item { get; set; }
        public string MenuItemId { get;set; }

	    private string _menuItemName;
		public string MenuItemName
		{
			get
			{
				if( _menuItemName == null && Item != null ) _menuItemName = Item.Name;
				return _menuItemName ?? string.Empty;
			}
			set { _menuItemName = value; }
		}

	    public MenuItem GetMenuItem()
        {
            return Item;
        }
    }
}
