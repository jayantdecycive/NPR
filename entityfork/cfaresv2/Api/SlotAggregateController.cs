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

namespace cfaresv2.Api
{
    

    public class SlotAggregateController : ApiController
    {
        SlotRepository repo;
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new SlotRepository(context);
            base.Initialize(controllerContext);
        }
        // GET api/reseventaggregate
        public IQueryable<SlotDayAggregate> Get()
        {
            return null;
        }

        // GET api/reseventaggregate/5
        public IQueryable<SlotDayAggregate> Get(int id)
        {
            
            
            var eventRepo = new ResEventRepository(repo.Context);
            
            IQueryable<Slot> dbQuery;
            string useEvent = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "event").Value;
            if(!string.IsNullOrEmpty(useEvent)){
                dbQuery = repo.Get(x => x.Occurrence.ResEventId == id, "Occurrence").Cast<Slot>();
            }else{
                dbQuery = repo.Get(x => x.OccurrenceId == id).Cast<Slot>();
            }
            
            var aggregate = Request.GetQueryNameValuePairs().FirstOrDefault(x => x.Key == "group");
            SlotGrouping grouping;
            switch (aggregate.Value)
            {
                
                case "day":
                    grouping = SlotGrouping.Day;
                    break;
                case "all":
                    grouping = SlotGrouping.All;
                    break;
                default:
                    grouping = SlotGrouping.Date;
                    break;
            }
            return repo.GetAggregates(grouping, dbQuery).Select(k => k.Key);
        }

        // POST api/reseventaggregate
        public void Post(int id, SlotDayAggregate value, DateTime cutoff, TimeSpan cutoffDistance, bool isEvent = false, SlotGrouping mode = SlotGrouping.Date)
        {
            if (isEvent)
            {
                var eventRepo = new ResEventRepository(repo.Context);
                IResEvent resEvent = eventRepo.Find(id);
                repo.AddAggregate(resEvent, mode, value, cutoff, cutoffDistance);
            }
            else {
                var occurrenceRepo = new OccurrenceRepository(repo.Context);
                IOccurrence occurrence = occurrenceRepo.Find(id);
                repo.AddAggregate(occurrence, mode, value, cutoff, cutoffDistance);
            }
        }

        // PUT api/reseventaggregate/5
        public void Put(int id, SlotDayAggregate oldValue, SlotDayAggregate value, DateTime cutoff, TimeSpan cutoffDistance, bool isEvent = false, SlotGrouping mode = SlotGrouping.Date)
        {
            if (isEvent)
            {
                var eventRepo = new ResEventRepository(repo.Context);
                IResEvent resEvent = eventRepo.Find(id);
                repo.UpdateAggregate(resEvent, mode, oldValue, value, cutoff, cutoffDistance);
            }
            else
            {
                var occurrenceRepo = new OccurrenceRepository(repo.Context);
                IOccurrence occurrence = occurrenceRepo.Find(id);
                repo.UpdateAggregate(occurrence, mode, oldValue, value, cutoff, cutoffDistance);
            }
        }

        // DELETE api/reseventaggregate/5
        public void Delete(int id)
        {
        }
    }
}
