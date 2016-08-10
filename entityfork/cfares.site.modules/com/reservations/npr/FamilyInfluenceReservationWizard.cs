﻿
#region Imports

using System;
using System.Collections.Generic;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;

#endregion

namespace cfares.site.modules.com.reservations.npr
{
    public class FamilyInfluenceReservationWizard : ReservationWizard
    {
        public FamilyInfluenceReservationWizard( ITicket t, string id ) : base( t, id ) {}
        public FamilyInfluenceReservationWizard( ITicket t, string id, string querystring ) : base(t, id, querystring) { }
    }
}