using cfares.domain._event;
using cfares.domain.aggregates;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using npr.domain._event.slot;
using cfares.domain._event.slot;

namespace cfaresv2.Api
{
    

    public class SlotServiceController : ApiController
    {
        SlotRepository repo;
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new SlotRepository(context);
            base.Initialize(controllerContext);
        }
        // GET api/reseventaggregate
        [HttpGet]
        public ICollection<ISlot> ByMonth(int month, int? year=null,string type=null)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;

            if (year == null)
                year = DateTime.Now.Year;

            if (type != null)
            {

                var query =
                    repo.Get(x => x.Start.Month == month && x.Start.Year == year && x.Occurrence.ResEvent.ReservationTypeId == type)
                        .Include("Occurrence.ResEvent").ToList();
                query.ForEach(x => x.Occurrence = null);
                return query;
            }
            else
                return repo.Get(x => x.Start.Month == month && x.Start.Year == year).Include("Tickets").ToList();
            //TODO - data contract resolver needs to be in place

        }



        [HttpGet]
        public ICollection<ISlot> ByStore(string storeId)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            var query = repo.Get(x => x.Occurrence.StoreId == storeId).ToList();           
            return query;
        }



        [HttpGet]
        public ICollection<NPRSlot> NPRByMonth(int month, int? year = null, string type = null)
        {
            var slotRepo = new SlotRepository<NPRSlot>(repo.Context);
            slotRepo.Context.Configuration.ProxyCreationEnabled = false;

            if (year == null) year = DateTime.Now.Year;
	        if (type == null)
		        return slotRepo.Get( x => 
					x.Start.Month == month && 
					x.Start.Year == year )
					.Include("Tickets").Cast<NPRSlot>().ToList();

			List<NPRSlot> slots = slotRepo.Get(x =>
		        x.Start.Month == month &&
		        x.Start.Year == year &&
		        x.Occurrence.ResEvent.ReservationTypeId == type &&
		        x.Visibility == SlotVisibility.Public)
			    .Include("Occurrence.ResEvent").Include("Tickets")
                .Cast<NPRSlot>().ToList();

			slots.RemoveAll( o => o.Occurrence.ResEvent.Status != ResEventStatus.Live );
			slots.ForEach( x => x.Occurrence = null );
		    return slots;
        }

        
    }
}
