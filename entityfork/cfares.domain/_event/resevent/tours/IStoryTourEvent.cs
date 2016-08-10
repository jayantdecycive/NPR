using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core.synchronization.Automation;
using cfares.domain._event.resevent.tours;

namespace cfares.domain._event.reservations
{
    [ITable]
    public interface IStoryTourEvent : ITourEvent
    {
    }
}
