using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using cfares.domain._event._ticket;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using System.Collections.Generic;
using System.Linq;
using cfares.domain._event;
using cfares.repository.ticket;
using cfares.site.modules.com.application;
using cfares.site.modules.user;
using System.Data.Entity;

namespace cfaresv2.Api
{
    public class FoodTicketProductSummaryController : ApiController
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

		[HttpGet]
        public HttpResponseMessage GetProductSummary( int slotId, string username )
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;
            IResContext context = ReservationConfig.GetContext();
            UserMembershipRepository ur = new UserMembershipRepository( context );
			ResUser owner = ur.GetUser( username );
	        UserOperationRole role = owner.OperationRole;

	        IQueryable<ITicket> query = GetAllQueryable(role, owner).Include("Owner")
		        .Include("Owner.Address").Include("Slot");

            if (role == UserOperationRole.Admin)
				#pragma warning disable 1717
                query = query;
				#pragma warning restore 1717

			else if (role == UserOperationRole.Operator)
		        query = query.Include("Slot.Occurrence.Store").Where(x => x.Slot.Occurrence.Store.OperatorId == owner.UserId||
		                                                                 x.OwnerId == owner.UserId);

			else query = query.Where(x => x.OwnerId == owner.UserId);

			IEnumerable<FoodTicket> tickets = query.Where( o => o.SlotId == slotId ).ToList().OfType<FoodTicket>();
			IEnumerable<FoodTicket> r = tickets.GroupBy( o => o.MenuItemId, o => o, 
				(key, g) => new FoodTicket { SlotId = slotId, AllocatedCapacity = g.Count(), 
					MenuItemId = key, MenuItemName = g.First().MenuItemName } ).Select( o => o );

	        return new HttpResponseMessage {
                Content = new JsonContent( r )
            };
        }

        protected IQueryable<ITicket> GetAllQueryable(UserOperationRole role,ResUser owner)
        {
            repo.Context.Configuration.ProxyCreationEnabled = false;

			IQueryable<ITicket> query = repo.GetAll().Include("Owner")
		        .Include("Owner.Address").Include("Slot");

            if (role == UserOperationRole.Admin)
                return query;

			if (role == UserOperationRole.Operator)
		        return query.Include("Slot.Occurrence.Store").Where(x => x.Slot.Occurrence.Store.OperatorId == owner.UserId||
		                                                                 x.OwnerId == owner.UserId);

			return query.Where(x => x.OwnerId == owner.UserId);
        }

    }

		public class ODataResponse
        {
            [JsonProperty("odata.metadata")]
            public string Metadata { get; set; }

            [JsonProperty("odata.count")]
            public string Count { get; set; }

			[JsonProperty("value")]
            public object Value { get; set; }
        }

public class JsonContent : HttpContent {

    private readonly MemoryStream _Stream = new MemoryStream();
    public JsonContent(object value) {

        var jw = new JsonTextWriter( new StreamWriter(_Stream));
        jw.Formatting = Formatting.Indented;
        var serializer = new JsonSerializer();
        serializer.Serialize(jw, value);
        jw.Flush();
        _Stream.Position = 0;

    }
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context) {
        return _Stream.CopyToAsync(stream);
    }

    protected override bool TryComputeLength(out long length) {
        length = _Stream.Length;
        return true;
    }
}

}
