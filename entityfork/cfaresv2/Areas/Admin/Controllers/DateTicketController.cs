using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using cfares.domain._event;
using cfares.domain._event._ticket.tours;
using cfares.domain._event.slot.tours;
using cfares.domain._event._tickets;
using cfares.entity.dbcontext.res_event;
using System.Data;
using cfares.repository.ticket;
using cfares.site.modules.com.Security;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class DateTicketController : CrudController<TicketRepository<DateTicket>, DateTicket,DateTicket, int,ITicket>
    {

        

        public override ITicket Inject(int id, DateTicket entity)
        {
            entity.TicketId = id;
            return entity;
        }


        protected override Dictionary<string, Action<DateTicket, string>> QueryInjector()
        {
            return new Dictionary<string,Action<DateTicket,string>>()
                {
                    {"SlotId",(x,y)=>x.SlotId=NullableInt(y)},
                    {"OwnerId",(x,y)=>x.OwnerId=NullableInt(y)},
                };
        }
        



        public override TicketRepository<DateTicket> GetRepository(IResContext context)
        {
            return new TicketRepository<DateTicket>(context);
        }
    }
}
