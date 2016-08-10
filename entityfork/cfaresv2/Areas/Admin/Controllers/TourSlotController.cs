using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
    public class TourSlotController : CrudController<SlotRepository<TourSlot>, TourSlot, TourSlot,int,ISlot>
    {

        protected override void FormValid(ISlot entity, TourSlot entityViewModel, FormCollection collection)
        {

            
            /*OccurrenceRepository occurrenceRepo = new OccurrenceRepository(serv.Context);
            entity.Occurrence = occurrenceRepo.Find(int.Parse(collection["Occurrence"])) as Occurrence;*/
            /*entity.Start = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_Start"]));
            entity.End = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Availability_End"]));
            entity.Cutoff = Occurrence.ConvertFromTimeZoneContext(entityViewModel.Occurrence, DateTime.Parse(collection["Cutoff"]));*/

        }



        public override ISlot Inject(int slotId, TourSlot entity)
        {
            return entity;
        }



        public override SlotRepository<TourSlot> GetRepository(IResContext context)
        {
            return new SlotRepository<TourSlot>(context);
        }
    }
}
