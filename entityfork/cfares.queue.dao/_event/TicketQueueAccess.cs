using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfares.domain._event;
using cfacore.queue.dao._base;

namespace cfares.queue.dao._event
{
    public class TicketQueueAccess : IQueueAccess<Ticket> 
    {
        public TicketQueueAccess() { 
        
        }
        public TicketQueueAccess(string connection) { 
        
        }

        public Ticket Load(string ID)
        {
            throw new NotImplementedException();
        }

        public Ticket Load(Uri URI)
        {
            throw new NotImplementedException();
        }

        public bool Save(Ticket obj)
        {
            throw new NotImplementedException();
        }

        public bool Save(Ticket obj, string action)
        {
            throw new NotImplementedException();
        }


        public bool Delete(Ticket obj)
        {
            throw new NotImplementedException();
        }

        public Ticket[] Search(KeyValuePair<string, string>[] criteria)
        {
            return null;
        }
    }
}
