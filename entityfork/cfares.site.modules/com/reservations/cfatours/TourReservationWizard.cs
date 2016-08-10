
#region Imports

using System;
using System.Collections.Generic;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain._event._ticket.tours;
using cfares.site.modules.com.reservations.res;

#endregion

namespace cfares.site.modules.com.reservations.cfatours
{
    public class TourReservationWizard : ReservationWizard<TourTicket>,IReservationWizard<TourTicket>
    {
        public TourReservationWizard() : base(null, null) { }
        public TourReservationWizard(TourTicket t, string id) : base(t, id) { }
        public TourReservationWizard(TourTicket t, string id, string querystring) : base(t, id, querystring) { }
    }
    
}
