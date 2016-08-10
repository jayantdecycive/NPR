using cfares.domain._event.slot.tours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cfares.domain.user
{
    public class ResAdmin:ResUser,IResAdmin
    {
        public ResAdmin(string ID) : base(ID) { 
        
        }

        public ICollection<TourSlot> GuideForSlots { get; set; }

        public ResAdmin()
            : base()
        {

        }
    }
}
