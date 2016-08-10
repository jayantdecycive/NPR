
#region Imports

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfaresv2.Areas.Admin.Controllers._base;
using npr.domain._event.slot;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class NPRSlotController : CrudController<SlotRepository<NPRSlot>, NPRSlot,NPRSlot, int,ISlot>
    {
		OccurrenceRepository _occurrenceServ;

		
		protected override void FormValid(ISlot entity, NPRSlot entityViewModel, FormCollection collection)
        {
			if( entity.ScheduleId == 0 ) entity.ScheduleId = null;
            
			collection.ValidateDateTimeOffset( ModelState, "StartOffsetString", "Availability Start" );
			collection.ValidateDateTimeOffset( ModelState, "EndOffsetString", "Availability End" );
			collection.ValidateDateTimeOffset( ModelState, "CutoffString", "Cutoff" );

            /*OccurrenceRepository occurrenceRepo = new OccurrenceRepository(serv.Context);
            entity.Occurrence = occurrenceRepo.Find(int.Parse(collection["Occurrence"])) as Occurrence;*/
            /*entity.Start = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_Start"]));
            entity.End = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_End"]));
            entity.Cutoff = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Cutoff"]));*/

        }

        protected override Dictionary<string, Action<NPRSlot, string>> QueryInjector()
        {
            return new Dictionary<string, Action<NPRSlot, string>> {
                {"OccurrenceId",(x,y)=>x.OccurrenceId=NullableInt(y)},
            };
        }

        public override ISlot Inject(int slotId, NPRSlot entity)
        {
			/*
            if (entity.OccurrenceId != null)
            {
                IOccurrence occurrence = _occurrenceServ.Find(entity.OccurrenceId.Value);
                entity.PrintBadge = occurrence.ResEvent.ReservationTypeId == "Tour";
            }
            else
                entity.PrintBadge = ViewBag.ReservationTypeId == "Tour";
			*/
            
			return entity;
			
        }

        public override SlotRepository<NPRSlot> GetRepository(cfares.entity.dbcontext.res_event.IResContext context)
        {
            return new SlotRepository<NPRSlot>(context);
        }

		protected override IAsyncResult BeginExecute(System.Web.Routing.RequestContext requestContext, AsyncCallback callback, object state)
		    {
			AdminController.AutoDetectSection( requestContext, ViewBag );

            var context = ReservationConfig.GetContext();
			_occurrenceServ = new OccurrenceRepository( context );

			return base.BeginExecute(requestContext, callback, state);
		}

		public override ActionResult Create()
		{
			ViewBag.OccurrenceId = Request.QueryString["OccurrenceId"].ToNullableInt();
			if( ViewBag.OccurrenceId != null )
			{
				IOccurrence occurrence = _occurrenceServ.Find( ViewBag.OccurrenceId );
				if( occurrence != null ) {
					ViewBag.ResEventId = occurrence.ResEventId;
					ViewBag.ReservationTypeId = occurrence.ResEvent.ReservationTypeId;
				}
			}

			return base.Create();
		}
    }
}
