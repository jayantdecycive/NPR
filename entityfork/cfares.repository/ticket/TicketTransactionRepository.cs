
#region Imports

using System.Collections.Generic;
using cfacore.domain.user;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
using System.Linq;

#endregion

namespace cfares.repository.ticket
{
    

    public class TicketTransactionRepository : GenericRepository<IResContext, TicketTransaction, string>
    {
        public TicketTransactionRepository(IResContext context) : base(context) { }

        public override IQueryable<TicketTransaction> PublicQuerySet()
        {
            return null;
        }

        public override TicketTransaction FindBySlug(string slug)
        {
            return Find(x => x.TicketTransactionId == slug);
        }

        

        public IQueryable<TicketTransaction> GetByTicket(Ticket ticket)
        {
            return Get(x=>x.TicketId==ticket.TicketId);
        }
    }
}
