using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cfares.repository.ticket
{
    public class TicketGuestsRepository : GenericRepository<IResContext, TicketGuests, string>
    {
        public TicketGuestsRepository(IResContext context) : base(context) { }

        public override TicketGuests FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TicketGuests> GetByTicket(Ticket ticket)
        {
            return Get(x => x.TicketId == ticket.TicketId);
        }
    }
}
