
#region Imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using cfacore.shared.modules.helpers;
using cfares.domain._event;
using cfares.domain.user;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.Security;
using cfares.site.modules.com.application;
using cfares.site.modules.repository.slot;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;

#endregion

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class SlotController : CrudController<ResSlotRepository, Slot, Slot,int,ISlot>
    {
        protected override void FormValid(ISlot entity, Slot entityViewModel, FormCollection collection)
        {
			if( entity.ScheduleId == 0 ) entity.ScheduleId = null;
            
            /*OccurrenceRepository occurrenceRepo = new OccurrenceRepository(serv.Context);
            entity.Occurrence = occurrenceRepo.Find(int.Parse(collection["Occurrence"])) as Occurrence;*/
            /*entity.Start = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_Start"]));
            entity.End = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_End"]));
            entity.Cutoff = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Cutoff"]));*/
        }

        protected override Dictionary<string, Action<Slot, string>> QueryInjector()
        {
            return new Dictionary<string, Action<Slot, string>> {
                {"OccurrenceId",(x,y)=>x.OccurrenceId=NullableInt(y)},
            };
        }

		public override ActionResult Create(Slot entity, FormCollection collection)
		{
			ActionResult r = base.Create(entity, collection);
			if( r is RedirectResult || r is RedirectToRouteResult )
                return RedirectToAction("Details", "Occurrence", new { id=entity.OccurrenceId });
			return r;
		}

        [HttpPost]
		public override ActionResult Edit(int id, Slot entity, FormCollection collection)
		{
            IResContext resContext = ReservationConfig.GetContext();
            ResSlotRepository lookupServ = GetRepository(resContext);            
		    ISlot slot = lookupServ.Get( o => o.SlotId == id ).FirstOrDefault();
		    IResEvent e = slot.Occurrence.ResEvent;
			
			if( collection["Capacity"].IsValid() ) slot.Capacity = collection["Capacity"].ToInt();
			if( ValidateDailyMinimumCapacity( ModelState, slot, e, false ) )
				return base.Edit(id, entity, collection);

	        return View( slot );
		}

        [HttpPost]
		public override ActionResult Delete(int id, FormCollection collection)
		{
            IResContext resContext = ReservationConfig.GetContext();
            ResSlotRepository lookupServ = GetRepository(resContext);            
		    ISlot slot = lookupServ.Get( o => o.SlotId == id ).FirstOrDefault();
		    IResEvent e = slot.Occurrence.ResEvent;

			if( collection["Capacity"].IsValid() ) slot.Capacity = collection["Capacity"].ToInt();
			if( ValidateDailyMinimumCapacity( ModelState, slot, e, true ) )
	 			 return base.Delete(id, collection);

	        return View( slot );
		}

		private bool ValidateDailyMinimumCapacity( ModelStateDictionary modelState, ISlot slot, IResEvent e, bool deletingSlot )
		{
            IResContext context = ReservationConfig.GetContext();
            UserMembershipRepository ur = new UserMembershipRepository( context );
			ResUser u = ur.GetUser();
			if( u == null ) { 				
				modelState.AddModelError( string.Empty, "Unable to delete slot - " + 
					"User not currently logged in.  Please login and try again" );
				return false;
			}

			// Uses slot's start time as the daily range check epoch
			bool errorConditionMet = deletingSlot ? 
				! slot.IsMinimumDailyCapacityMetExcludingThisSlot : 
				! slot.IsMinimumDailyCapacityMet;

			// Admin's can override this check
			if( u.OperationRole != UserOperationRole.Admin && errorConditionMet ) 
			{ 
				modelState.AddModelError( string.Empty, string.Format( "Unable to delete slot - " + 
					"The minimum daily capacity of {0} is not met [ Capacity for {2} = {1} ]",
					e.MinimumDailyCapacity, ( deletingSlot ? 
						slot.CurrentDailyCapacityExcludingThisSlot : 
						slot.CurrentDailyCapacity ), slot.Start.ToDateStringWithDayOfWeek() ) );

				return false;
			}
			return true;
		}

		//
        // GET: /Admin/TEntity/Delete/5

        public virtual ActionResult Destroy(string ids)
        {
            int[] _ids = ids.Split(',').Select(int.Parse).ToArray();
            var entities = serv.Get(x => _ids.Contains(x.SlotId));
            CheckDeleteRights(entities.ToList());
            return View(entities.ToList());
        }

       
        [HttpPost]
        public virtual ActionResult Destroy(string ids, FormCollection collection)
        {
            int[] _ids = ids.Split(',').Select(int.Parse).ToArray();
            var entities = serv.Get(x => _ids.Contains(x.SlotId));
            CheckDeleteRights(entities.ToList());
            try
            {
                // TODO: Add delete logic here
                foreach(var entity in entities)
                    serv.Delete(entity);
                serv.Commit();
                return RedirectToAction("Index", new { message = "Those items have been deleted" });
            }
            catch(Exception)
            {
				ModelState.AddModelError( string.Empty, "Unable to delete one or more of the selected slots due to tickets existing for those slots.  Please ensure no tickets exist for the slots being deleted." );
                return View(entities.ToList());
            }
        }

        public override ISlot Inject(int slotId, Slot entity)
        {
            return entity;
        }

        public override ResSlotRepository GetRepository(IResContext context)
        {
            return new ResSlotRepository(context);
        }
    }
}
