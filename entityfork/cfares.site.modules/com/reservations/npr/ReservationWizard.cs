
#region Imports

using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using cfacore.shared.modules.com.admin;
using cfares.domain._event;

#endregion

namespace cfares.site.modules.com.reservations.npr
{
    public class ReservationWizard : Wizard<ITicket>, IReservationWizard
    {
        public ReservationWizard(ITicket t, string id, string querystring) : base(t, id)
        {
            Qs = ToDictionary(querystring);
        }
        protected IDictionary<string, string> Qs;

        public string Q
        {
            get { return ToQueryString(Qs); }
        }

        public ReservationWizard(ITicket t, string id) : base(t, id)
        {
            Qs = new Dictionary<string, string>();
        }

		public static string FirstStepName
		{ 
			get { return new ReservationWizard( null, null ).FirstStep.Name; }
		}

		public static string ModifyStepName
		{
            get { return new ReservationWizard(null, null).GetStep("Reservation.Register").Name; }
		}

		// TODO - SH - Remove hardcode found added to wizard profile
        
        public override IWizard<ITicket> Prime( ITicket t )
        {
			if( t == null ) return null;

            string id = t.Id() ?? "";

            IResEvent res = null;
            if (t.Slot != null && t.Slot.OccurrenceId != null&&t.Slot.Occurrence.ResEventId!=null)
                res = (t.Slot.Occurrence.ResEvent);
            Prime(res);
            
            IOccurrence oc = null;
            if(t.Slot!=null && t.Slot.OccurrenceId!=null)
                oc=(t.Slot.Occurrence);
            Prime(oc);

            ApplyComplete("Reservation.Tours", true).WithRoute(new RouteValueDictionary()
            { { "Action", "Tours" },
                {"id", id},
                {"Controller", "Home"}
            }, Q);
            ApplyComplete("Reservation.Registration", false).WithRoute("Register", id, Q);
			ApplyComplete("Reservation.Payment", false).WithRoute("Payment", id, Q);
			ApplyComplete("Reservation.Review", false).WithRoute("Overview", id, Q);
            ApplyComplete("Reservation.Success", false).WithRoute("Confirm", id, Q);

			return this;
        }

        protected string ToQueryString(IDictionary<string,string> nvc)
        {
	        if (nvc == null) return string.Empty;
            return "?" + string.Join("&", Array.ConvertAll(
				nvc.Keys.ToArray(), 
					key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        }

        protected IDictionary<string, string> ToDictionary(string qs)
        {
            var collec = HttpUtility.ParseQueryString(qs);
            return collec.AllKeys.ToDictionary(k=>k,k=>collec[k]);
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

        public IReservationWizard Prime(IOccurrence occurrence)
        {
            if (occurrence != null && Qs != null)
                Qs["occurrence"] = occurrence.OccurrenceId.ToString(CultureInfo.InvariantCulture);
            ApplyComplete("Reservation.Registration", false).WithRoute("Register", (Model == null ? "" : Model.Id()), Q);
            return this;
        }

        public IReservationWizard Prime(IResEvent _event)
        {
            if (_event!=null && Qs!=null)
                Qs["event"] = _event.ResEventId.ToString(CultureInfo.InvariantCulture);
            
            return this;
        }

        public IReservationWizard Prime(ISlot slot)
        {
            return this;
        }

        public override List<WizardStep> GetEmptySteps()
        {
            return new List<WizardStep>
	        {
		        new WizardStep {Name = "Details", Key = "Reservation.Tours"},
		        new WizardStep {Name = "Register", Key = "Reservation.Registration"},
				new WizardStep {Name = "Payment", Key = "Reservation.Payment"},
		        new WizardStep {Name = "Overview", Key = "Reservation.Review"},
		        new WizardStep {Name = "Confirmation", Key = "Reservation.Success"},
	        };
        }
    }
}
