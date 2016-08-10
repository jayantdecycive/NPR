using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.domain._base;

namespace cfares.domain._event
{
    public interface IMetaTicketCollection<Q>:IList<Q> where Q:Ticket,new()
    {
    }
}
