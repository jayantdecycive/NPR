using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain._event.menu;
using cfares.domain._event._ticket;
using cfares.domain._event.resevent.store;
using cfares.domain.store;
using cfares.repository.menuitem;
using cfares.repository.schedule;
using cfares.repository.store;
using cfares.site.modules.com.application;

namespace cfares.site.modules.com.reservations.res
{
    public class ReceptionWizard : ReservationWizard<GuestTicket>, IReservationWizard<GuestTicket>
    {

        public ReceptionWizard(GuestTicket model, string id)
            : base(model, id)
        {
        }





        public override IWizard<GuestTicket> Prime(GuestTicket t)
        {
            if (t == null) return null;

            string id = t.Id() ?? "";

            IResEvent res = null;
            if (t.Slot != null && t.Slot.OccurrenceId != null && t.Slot.Occurrence.ResEventId != null)
                res = (t.Slot.Occurrence.ResEvent);
            Prime(res);
            
            IOccurrence oc = null;
            if (t.Slot != null && t.Slot.OccurrenceId != null)
                oc = (t.Slot.Occurrence);
            Prime(oc);

            ISlot evtSlot=null;
            if(res!=null)
            evtSlot= ((SpeakerEvent) res).GetSlot();
            if (evtSlot != null)
                Prime(evtSlot);

            ApplyComplete("Reservation.ReceptionList", t.Slot != null && t.Slot.OccurrenceId != null && evtSlot!=null).WithRoute("Receptions", id, Q);
            ApplyComplete("Reservation.Review", false).WithRoute("Review", id, Q);
            ApplyComplete("Reservation.Success", false).WithRoute("Success", id, Q);
            ApplyComplete("Reservation.Reservation", false).WithRoute("Reservation", id, Q);
            return this;
        }

        public override IReservationWizard Prime(IResEvent res)
        {
            ISlot evtSlot = null;
            //if (res != null)
              //  evtSlot = ((SpeakerEvent)res).GetSlot();
            if (evtSlot != null)
                Prime(evtSlot);
            ApplyComplete("Reservation.ReceptionList", evtSlot != null);
            return base.Prime(res);
        }

        /*public IWizard<FoodTicket> Prime(MenuItem model)
        {
            if (model != null)
                this.FoodId = model.DomId;

            ApplyComplete(NextStep, false).WithQuery(Q);
            return this;
        }*/

        public string ScheduleId
        {
            get
            {
                if (!Qs.ContainsKey("schedule"))
                    return null;
                return (Qs["schedule"]);
            }
            set { Qs["schedule"] = value; }
        }

        

        public Schedule GetSchedule()
        {
            if (this.ScheduleId != null)
            {
                var foodRepo = new ScheduleRepository(ReservationConfig.GetContext());
                return foodRepo.Find(int.Parse(this.ScheduleId));
            }
            
            return null;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "Reception List", Key = "Reservation.ReceptionList"},
		        //new WizardStep {Name = "Location", Key = "Reservation.Location"},
                new WizardStep {Name = "Reservation", Key = "Reservation.Reservation"},
		        new WizardStep {Name = "Review", Key = "Reservation.Review"},
		        new WizardStep {Name = "Success", Key = "Reservation.Success"},
	        };
        }

        public override void ApplyContext(System.Web.Routing.RequestContext context)
        {
            
            var routeDict = context.HttpContext.Request.QueryString.AllKeys.Where(x => !Qs.ContainsKey(x)).Select(x => new { Key = x, Value = context.HttpContext.Request.QueryString[x] })
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var kvp in routeDict)
                Qs.Add(kvp.Key, kvp.Value);

            //ApplyComplete("Reservation.Food", !string.IsNullOrEmpty(ScheduleId)).WithQuery(Q);
            //ApplyComplete("Reservation.Calendar", SlotId!=null).WithQuery(Q);
        }
        

    }
}
