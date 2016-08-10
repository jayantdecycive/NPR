using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfares.domain._event.resevent.store;

namespace cfares.domain._event.menu
{
    public class MenuItemAllowance
    {
        public virtual GiveawayEvent ResEvent { get; set; }
        public int? ResEventId { get; set; }

        public virtual MenuItem AllowedItem { get; set; }
        public string AllowedItemId { get; set; }

        public MenuItemCondition Condition { get; set; }
    }

    public enum MenuItemCondition
    {
        [Description("Available, On by Default")]
        DefaultOn,
        [Description("Available, Off by Default")]
        DefaultOff,
        [Description("Available, Must be On")]
        LockedOn
    }
}
