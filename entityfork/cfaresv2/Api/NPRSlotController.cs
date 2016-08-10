
#region Imports

using System;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Net.Http;
using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System.Linq;
using cfares.repository.slot;
using cfaresv2.Api._base;
using System.Data.Entity;
using npr.domain._event.slot;

#endregion

namespace cfaresv2.Api
{
    public class NPRSlotController : RepositoryEntitySetController<SlotRepository<NPRSlot>, NPRSlot, int,ISlot>
    {
        protected override SlotRepository<NPRSlot> GetRepository(IResContext context, 
            System.Web.Http.Controllers.HttpControllerContext httpController = null)
        {
            return new SlotRepository<NPRSlot>(context);
        }

        protected override IQueryable<ISlot> GetAllQueryable(UserOperationRole role, ResUser owner)
        {
            IQueryable<ISlot> query = base.GetAllQueryable(role).Include("Guide").Include("Tickets")
				.Include("Occurrence.Store").Include("Occurrence.ResEvent.Category");

            if (Request.GetQueryNameValuePairs().Any(x => x.Key == "day"))
            {

                DayOfWeek day = (DayOfWeek)int.Parse(Request.GetQueryNameValuePairs().First(x => x.Key == "day").Value);
                var d = (int)day;
                query =
                    (query).Include("Occurrence").Where(x => SqlFunctions.DatePart("dw", x.Start) == d);
            }

            if (role == UserOperationRole.Admin)
                return query;

	        if (role == UserOperationRole.Operator)
		        return query.Include("Occurrence.Store").Where(x => x.Occurrence.Store.OperatorId == owner.UserId);

            

			return query;
        }

        protected override IQueryable<ISlot> GetPublicQueryable()
        {
            return new Collection<NPRSlot>().AsQueryable().Include("Guide").Include("Tickets");
        }
    }
}
