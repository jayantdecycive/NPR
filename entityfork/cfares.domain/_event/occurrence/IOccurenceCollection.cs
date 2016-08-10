using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event.slot;

namespace cfares.domain._event
{
    public interface IOccurrenceCollection:IList<Occurrence>
    {
        SlotCollection Slots
        {
            get;            
        }
    }
}
