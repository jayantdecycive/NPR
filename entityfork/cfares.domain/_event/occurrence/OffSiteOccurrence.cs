using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacore.shared.domain.user;

namespace cfares.domain._event.occ
{
    public class OffSiteOccurrence: Occurrence
    {

        public int? OffSiteAddressId { get; set; }
        public virtual Address OffSiteAddress { get; set; }
        public string OffSiteDescription { get; set; }
    }
}
