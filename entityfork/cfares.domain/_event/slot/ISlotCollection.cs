using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event.slot.tours;

namespace cfares.domain._event
{
    public interface ISlotCollection<Q> : IList<Q> where Q : Slot,new()
    {
    }

}
