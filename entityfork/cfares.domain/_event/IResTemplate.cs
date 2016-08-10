using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using core.synchronization.Automation;
using cfacore.shared.domain.media;
using System.Data.Linq.Mapping;
using cfacore.domain._base;
using cfacore.shared.domain.attributes;
using System.ComponentModel.DataAnnotations;

namespace cfares.domain._event
{
    [ITable]    
    public interface IResTemplate : IDomainObject
    {
                
        Media Preview { get; set; }

        string Description { get; set; }

        string Name { get; set; }

        string BrowserMedia { get; set; }

        string DefaultReservationTypeId { get; set; }

        string ResTemplateId { get; set; }
    }
}
 