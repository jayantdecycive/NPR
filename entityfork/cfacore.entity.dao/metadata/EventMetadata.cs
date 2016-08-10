using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace cfacore.entity.dao._event
{
    [MetadataType(typeof(cfares.domain._event.slot.SlotMeta))]
    public partial class Slot{    }

    [MetadataType(typeof(cfares.domain._event.slot.ITourSlot))]
    public partial class Slot_Tour
    {

    }

    [MetadataType(typeof(cfares.domain._event.ITicket))]
    public partial class Ticket { 
    
    }

    [MetadataType(typeof(cfares.domain._event.slot.ITourTicket))]
    public partial class Ticket_Tour
    {

    }

    [MetadataType(typeof(cfares.domain._event.IResEvent))]
    public partial class ResEvent
    {

    }


    [MetadataType(typeof(cfares.domain._event.IOccurrence))]
    public partial class Occurrence
    {

    }

    [MetadataType(typeof(cfacore.domain.store.IStore))]
    public partial class Location
    {

    }

    [MetadataType(typeof(cfacore.domain.user.IAddress))]
    public partial class Address
    {

    }

    [MetadataType(typeof(cfacore.shared.domain.media.IMedia))]
    public partial class Medium
    {

    }

    [MetadataType(typeof(cfacore.shared.domain.store.IDistributor))]
    public partial class Distributor
    {

    }
    
    
}
