using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.attributes;

namespace cfares.domain._event
{
    
    public interface IReservationPlatform
    {
        IReservationTypeCollection Reservations { get; set; }
    }
}
