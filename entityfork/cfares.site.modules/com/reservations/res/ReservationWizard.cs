
#region Imports

using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;
using cfares.domain.store;
using cfares.repository._event;
using cfares.repository.slot;
using cfares.repository.store;
using cfares.site.modules.com.application;

#endregion

namespace cfares.site.modules.com.reservations.res
{
    public class ReservationWizard : ReservationWizard<ITicket>
    {
        public ReservationWizard(ITicket t, string id, string querystring)
            : base(t, id, querystring)
        {

        }



        public ReservationWizard(ITicket t, string id)
            : base(t, id)
        {

        }
    }

    
    public class ReservationWizard<TTicket> : Wizard<TTicket>, IReservationWizard<TTicket> where TTicket : class,ITicket
    {
        public ReservationWizard(TTicket t, string id, string querystring)
            : base(t, id, querystring)
        {
            
        }

        public string Q
        {
            get { return ToQueryString(Qs); }
        }


        public ReservationWizard(TTicket t, string id)
            : base(t, id)
        {
            
        }

        public ResStore GetStore()
        {
            if (this.OccurrenceId != null)
            {
                var locationRepo = new LocationRepository(ReservationConfig.GetContext());
                return locationRepo.FindOrRememberByOccurrence(this.OccurrenceId.Value);
            }
            else if (Model != null && Model.GetStore() != null)
            {
                return Model.GetStore();
            }
            return null;
        }

        public IOccurrence GetOccurrence()
        {
            if (this.OccurrenceId != null)
            {
                var occurrenceRepo = new OccurrenceRepository(ReservationConfig.GetContext());
                return occurrenceRepo.FindOrRemember(this.OccurrenceId.Value);
            }
            else if (Model != null && Model.GetOccurrence() != null)
            {
                return Model.GetOccurrence();
            }
            return null;
        }
     

        public virtual ISlot GetSlot()
        {
            if (this.SlotId != null)
            {
                var slotRepo = new SlotRepository(ReservationConfig.GetContext());
                return slotRepo.FindOrRemember(this.SlotId.Value);
            }
            else if (Model != null && Model.GetSlot() != null)
            {
                return Model.GetSlot();
            }
            return null;
        }

        public virtual IResEvent GetEvent()
        {
            var slotRepo = new ResEventRepository(ReservationConfig.GetContext());
            if (this.EventId != null)
            {

                return slotRepo.FindOrRemember(this.EventId.Value);
            }
            else if (Model != null && Model.GetOccurrence() != null)
            {
                return (Model.GetOccurrence().ResEvent);
            }
            return null;
        }

        public static string FirstStepName
        {
            get { return new ReservationWizard<TTicket>(null, null).FirstStep.Name; }
        }

        public static string ModifyStepName
        {
            get { return new ReservationWizard<TTicket>(null, null).GetStep("Reservation.Reservation").Name; }
        }

        // TODO - SH - Remove hardcode found added to wizard profile

        public override IWizard<TTicket> Prime(TTicket t)
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

            ApplyComplete("Reservation.SearchByLocation", t.Slot!=null&&t.Slot.OccurrenceId!=null).WithRoute("SearchByLocation", id, Q);
            ApplyComplete("Reservation.Review", false).WithRoute("Review", id, Q);
            ApplyComplete("Reservation.Success", false).WithRoute("Success", id, Q);
            ApplyComplete("Reservation.Reservation", false).WithRoute("Reservation", id, Q);
            return this;
        }

        

        public int? OccurrenceId
        {
            get
            {
                if (!Qs.ContainsKey("occurrence"))
                    return null;
                return int.Parse(Qs["occurrence"]);
            }
        }

        public int? SlotId
        {
            get
            {
                if (!Qs.ContainsKey("slot"))
                    return null;
                return int.Parse(Qs["slot"]);
            }
            set { Qs["slot"] = value.ToString(); }
        }

        public bool? IsFavorite
        {
            get
            {
                if (!Qs.ContainsKey("favorite"))
                    return null;
                return bool.Parse(Qs["favorite"]);
            }
            set { Qs["favorite"] = value.ToString(); }
        }

        public int? EventId
        {
            get
            {
                if (!Qs.ContainsKey("event"))
                    return null;
                return int.Parse(Qs["event"]);
            }
            set { Qs["event"] = value.ToString(); }
        }


        public virtual IReservationWizard Prime(IOccurrence occurrence)
        {
            if (occurrence != null)
                Qs["occurrence"] = occurrence.OccurrenceId.ToString(CultureInfo.InvariantCulture);
            ApplyComplete(NextStep, false).WithQuery(Q);
            return this;
        }

        public virtual IReservationWizard Prime(IResEvent _event)
        {
            if (_event != null)
                Qs["event"] = _event.ResEventId.ToString(CultureInfo.InvariantCulture);
            ApplyComplete(NextStep, false).WithQuery(Q);
            return this;
        }

        public virtual IReservationWizard Prime(ISlot slot)
        {
            if (slot != null)
                Qs["slot"] = slot.SlotId.ToString(CultureInfo.InvariantCulture);
            ApplyComplete(NextStep, true).WithQuery(Q);
            return this;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "SearchByLocation", Key = "Reservation.SearchByLocation"},
		        new WizardStep {Name = "Reservation", Key = "Reservation.Reservation"},
		        new WizardStep {Name = "Review", Key = "Reservation.Review"},
		        new WizardStep {Name = "Success", Key = "Reservation.Success"},
	        };
        }

        public virtual void ApplyContext(RequestContext context)
        {
            var routeDict = context.HttpContext.Request.QueryString.AllKeys.Where(x => x!=null&&!Qs.ContainsKey(x)).Select(x => new { Key = x, Value = context.HttpContext.Request.QueryString[x] })
                .ToDictionary(x => x.Key, x => x.Value);
            foreach (var kvp in routeDict)
                Qs.Add(kvp.Key, kvp.Value);

            ApplyComplete("Reservation.SearchByLocation", OccurrenceId != null).WithQuery(Q);
        }
    }
}
