using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace cfaresv2.Api
{
    public class TicketServiceController : ApiController
    {
        TicketRepository repo;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new TicketRepository(context);
            base.Initialize(controllerContext);
        }

        [HttpGet]
        public ICollection<Ticket> BySlot( int slotId )
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            return repo.Get( o => o.SlotId == slotId ).Include("Slots").Cast<Ticket>().ToList();
        }

       /* [HttpGet]
        public TicketTransaction Redeem(string CardNumber, int ResEventId)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            var t = repo.GetAll().Include("Slot.Occurrence").FirstOrDefault(o => o.CardNumber == CardNumber && o.Slot.Occurrence.ResEventId == ResEventId);

        }*/
    }
}
