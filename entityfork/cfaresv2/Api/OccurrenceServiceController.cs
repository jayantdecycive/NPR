using System.Collections;
using cfares.domain._event;
using cfares.entity.dbcontext.res_event;
using cfares.repository._event;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System;
using cfares.domain._event._ticket;
using npr.domain._event.ticket;

namespace cfaresv2.Api
{
    public class OccurrenceServiceController : ApiController
    {
        OccurrenceRepository repo;
        TicketRepository ticketRepo;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            IResContext context = ReservationConfig.GetContext();
            repo = new OccurrenceRepository(context);
            ticketRepo = new TicketRepository(context);
            base.Initialize(controllerContext);
        }

        [HttpGet]
        public ICollection<OccurrenceApiAppView> ByResEventType(string resEventTypeId, bool allEvents = false, bool hiddenEvents = false)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            DateTime cutoff = DateTime.Now.AddDays(-1);

            List<IOccurrence> olist = repo.Get(x =>
                    (x.ResEvent.ReservationTypeId == resEventTypeId) &&
                    (x.ResEvent.Status == ResEventStatus.Live || (hiddenEvents == true && x.ResEvent.Status == ResEventStatus.Hidden)) &&
                    (x.ResEvent.RegistrationStart > cutoff || allEvents)).Include("ResEvent").Include("ResEvent.Media").Include("SlotsList").Include("Store").ToList();


            return olist.Select(x => new OccurrenceApiAppView()
            {
                ResEventId = x.ResEventId,
                Name = x.ResEvent.Name.Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"'),
                SubHeading = x.ResEvent.SubHeading.Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"'),
                RegistrationStart = x.ResEvent.RegistrationStart.ToString(),
                MediaUrl = x.ResEvent.MediaUrl,
                MaximumCapacity = x.ResEvent.TotalCapacity,
                StoreName = x.Store.Name,
                Date = (x.SlotsList.FirstOrDefault() == null ? new DateTime() : x.SlotsList.FirstOrDefault().Start),
                DateString = (x.SlotsList.FirstOrDefault()==null ? "" : x.SlotsList.FirstOrDefault().Start.ToString("MMMM dd, yyyy | h:mm tt")),
                TicketsReservedCount = ticketRepo.Get(y => y.Slot.Occurrence.ResEventId == x.ResEventId).Where(z => z.Status==TicketStatus.Reserved).Count(),
                
                TicketsReserved = ticketRepo
                    .Get(y => y.Slot.Occurrence.ResEventId == x.ResEventId)
                    .Where(z => z.Status == TicketStatus.Reserved)
                    .Select(y => (y as NPRTicket) == null ? 1 : (y as NPRTicket).GroupSize)
                    .DefaultIfEmpty(1).Sum(),
            })
            .OrderBy(x => x.Date.Date)
            .ThenBy(x => x.Date.TimeOfDay)       
            .ToList();
        }


    }

    public class OccurrenceApiAppView
    {

        public int? ResEventId { get; set; }

        public string Name { get; set; }

        public string SubHeading { get; set; }

        public string RegistrationStart { get; set; }

        public string MediaUrl { get; set; }

        public int MaximumCapacity { get; set; }

        public int TicketsReservedCount { get; set; }
        
        public int TicketsReserved { get; set; }

        public string StoreName { get; set; }

        public string DateString { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}
