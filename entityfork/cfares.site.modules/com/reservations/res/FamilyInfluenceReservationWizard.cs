
#region Imports

using System;
using System.Collections.Generic;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain._event._tickets;

#endregion

namespace cfares.site.modules.com.reservations.res
{
    public class FamilyInfluenceReservationWizard : ReservationWizard<DateTicket>, IReservationWizard<DateTicket>
    {
        public FamilyInfluenceReservationWizard(DateTicket t, string id) : base(t, id) { }
        public FamilyInfluenceReservationWizard(DateTicket t, string id, string querystring) : base(t, id, querystring) { }


     
    }
}
