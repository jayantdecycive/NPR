using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using cfacore.domain._base;
using cfares.domain._event;

using cfares.domain._event.slot.tours;

using cfares.domain.user;
using cfares.domain._event.slot;
using cfares.entity.dbcontext.res_event;
using cfares.site.modules.com.Security;
using cfares.site.modules.user;
using cfaresv2.Areas.Admin.Controllers._base;
using cfares.repository.slot;
using cfares.repository._event;

namespace cfaresv2.Areas.Admin.Controllers
{
    [ReservationSystemAuthorize( Area = "Admin", Roles = "Admin,Operator" )]
    public class GiveawaySlotController : CrudController<GiveawaySlotRepository, GiveawaySlot, GiveawaySlot, int, ISlot>
    {
        protected override void FormValid(ISlot entity, GiveawaySlot entityViewModel, FormCollection collection)
        {
			if( entity.ScheduleId == 0 ) entity.ScheduleId = null;
            
            /*OccurrenceRepository occurrenceRepo = new OccurrenceRepository(serv.Context);
            entity.Occurrence = occurrenceRepo.Find(int.Parse(collection["Occurrence"])) as Occurrence;*/
            /*entity.Start = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_Start"]));
            entity.End = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_End"]));
            entity.Cutoff = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Cutoff"]));*/

        }

		public override ActionResult Create(GiveawaySlot entity, FormCollection collection)
		{
			ActionResult r = base.Create(entity, collection);
			if( r is RedirectResult || r is RedirectToRouteResult )
                return RedirectToAction("Details", "Occurrence", new { id=entity.OccurrenceId });
			return r;
		}

        protected override Dictionary<string, Action<GiveawaySlot, string>> QueryInjector()
        {
            return new Dictionary<string, Action<GiveawaySlot, string>>()
                {
                    {"OccurrenceId",(x,y)=>x.OccurrenceId=NullableInt(y)},
                };
        }

        //
        // GET: /Admin/TEntity/Delete/5

        public virtual ActionResult Destroy(string ids)
        {
            int[] _ids = ids.Split(',').Select(x => int.Parse(x)).ToArray();
            var entities = serv.Get(x => _ids.Contains(x.SlotId));
            CheckDeleteRights(entities.ToList());
            return View(entities.ToList());
        }

       
        [HttpPost]
        public virtual ActionResult Destroy(string ids, FormCollection collection)
        {
            int[] _ids = ids.Split(',').Select(x => int.Parse(x)).ToArray();
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
            catch( Exception ex )
            {
				ModelState.AddModelError( string.Empty, "Unable to delete one or more of the selected slots due to tickets existing for those slots.  Please ensure no tickets exist for the slots being deleted." );
                return View(entities.ToList());
            }
        }



        public override ISlot Inject(int slotId, GiveawaySlot entity)
        {
            return entity;
        }



        public override GiveawaySlotRepository GetRepository(IResContext context)
        {
            return new GiveawaySlotRepository(context);
        }
    }
}
