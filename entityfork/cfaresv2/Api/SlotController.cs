using System.Data.Objects.SqlClient;
using cfacore.shared.domain.store;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;
using cfares.repository.store;
using cfacore.shared.domain.media;
using cfacore.shared.modules.repository;
using cfares.domain._event;
using cfares.repository.slot;
using cfares.site.modules.com.application;
using cfares.site.modules.user;
using cfaresv2.Api._base;
using System.Data.Entity;

namespace cfaresv2.Api
{


    public class SlotController : RepositoryEntitySetController<SlotRepository, Slot, int,ISlot>
    {

        protected override SlotRepository GetRepository(IResContext context,
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new SlotRepository(context);
        }

        protected override IQueryable<ISlot> GetAllQueryable(UserOperationRole role)
        {
            var slots = base.GetAllQueryable(role)
				.Include("Schedule").Include("Tickets").Include("Occurrence");

            if (Request.GetQueryNameValuePairs().Any(x => x.Key == "event"))
            {
                int eventId = int.Parse(Request.GetQueryNameValuePairs().First(x => x.Key == "event").Value);
                slots=
                    (slots).Where(x => x.Occurrence.ResEventId == eventId);
            }
            if (Request.GetQueryNameValuePairs().Any(x => x.Key == "day"))
            {
                DayOfWeek day = (DayOfWeek)int.Parse(Request.GetQueryNameValuePairs().First(x => x.Key == "day").Value);
                var d = (int) day;
                slots =
                    (slots).Where(x => SqlFunctions.DatePart("dw",x.Start) == d);
            }
            if (Request.GetQueryNameValuePairs().Any(x => x.Key == "stores"))
            {
                string[] stores = (Request.GetQueryNameValuePairs().First(x => x.Key == "stores").Value).Split(',');
                slots = (slots).ToList().AsQueryable()
						.Where(x => stores.Contains(x.Occurrence.StoreId));
            }
            if (ReservationConfig.GetConfig().ApplicationId == Application.NPR)
            {
                slots = slots.Include("Occurrence.ResEvent.Category").Include("Occurrence.Store");
            }
            return slots;
        }

        protected override IQueryable<ISlot> GetPublicQueryable()
        {
            return base.GetPublicQueryable().Include("Tickets");
        }

        
    }
}
