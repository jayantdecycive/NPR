using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using cfacore.shared.domain.attributes;
using cfares.domain._event;
using System.ComponentModel.DataAnnotations;
using cfares.domain.user;
using npr.domain._event.ticket;
using cfares.domain._event.slot;

namespace npr.domain._event.slot
{
    [DataContract]
    [Table("NPRSlots")]
    [SyncUrl("/api/NPRSlot")]
    [ModelId("SlotId")]
    public class NPRSlot : Slot, INPRSlot,IBadgeSlot
    {

		public NPRSlot()
		{
			this.PrintBadge = true;
		}

	    [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "OwnerId", OtherKey = "UserId")]
        [DataType("AutoComplete/_User")]
        [UIHint("AdminLinkLoadable")]
        public virtual ResUser Guide { get; set; }

		[DataMember]
        [Column]
        [ClientDefault(0,typeof(int))]
        [DataType("AutoComplete/_UserId")]
        [Display(Name = "Guide")]
        public int? GuideId { get; set; }

		[DataMember]
		[Column]
		//[ClientDefault(true, typeof(bool))]
		[Display(Name = "Print Badge")]
		public bool PrintBadge
		{
			get
			{
				return this._printBadge;
			}
			set 
			{
				if (this.Occurrence != null && Occurrence.ResEvent.ReservationTypeId == "Tour") this._printBadge = true;
				else this._printBadge = value; 
			}
		}
		private bool _printBadge;



	    #region Passthrough fields for API access

		[DataMember]
        [Column]
        [Display(Name = "Event Type")]
        public string ReservationTypeId { get
        {
			if( Occurrence == null || Occurrence.ResEvent == null ) return string.Empty;
	        return Occurrence.ResEvent.ReservationTypeId;
        } 
		set { } }

		[DataMember]
        [Column]
        [Display(Name = "Event Name")]
        public string EventName { get
        {
			if( Occurrence == null || Occurrence.ResEvent == null ) return string.Empty;
	        return Occurrence.ResEvent.Name;
        } 
		set { } }

		[DataMember]
        [Column]
        [Display(Name = "Event Category")]
        public string EventCategory { get
        {
			if( Occurrence == null || Occurrence.ResEvent == null || Occurrence.ResEvent.Category == null ) return string.Empty;
	        return Occurrence.ResEvent.Category.Name;
        } 
		set { } }

		[DataMember]
        [Column]
        [Display(Name = "Event Status")]
        public string EventStatus { get
        {
			if( Occurrence == null || Occurrence.ResEvent == null ) return string.Empty;
	        return Occurrence.ResEvent.Status.ToString();
        } 
		set { } }

		[DataMember]
        [Column]
        [Display(Name = "Event Location Name")]
        public string EventLocationName { get
        {
			if( Occurrence == null || Occurrence.Store == null ) return string.Empty;
	        return Occurrence.Store.Name;
        } 
		set { } }

       

        public override int TicketsAvailable
        {
            get
            {
                if (IssuedTickets == null) return Capacity; 
                return Capacity - IssuedTickets.Cast<NPRTicket>().Sum(x=>x.GroupSize);
            }
            set
            {
                //base.TicketsAvailable = value;
            }
        }

        [DataMember]
        [Display(Name = "Total Tickets")]
        public override int TotalTickets
        {
            get
            {
                if (Tickets == null) return _TotalTicketCache;
                return IssuedTickets.Cast<NPRTicket>().Sum(x=>x.GroupSize);
            }
            set
            {
                if (Tickets == null) _TotalTicketCache = value;
            }
        }

		[DataMember]
		[Column]
		[Display(Name = "Start Day")]
		public string StartDay
		{
			get
			{
				return this.Start.DayOfWeek.ToString();
			}
			set
			{

			}
		}

		[DataMember]
		[Column]
		[Display(Name = "Start As String")]
		public string StartAsString
		{
			get
			{
				return this.Start.ToString("dddd MMMM dd, yyyy h:mm tt | MM/dd/yyyy h:mm tt");
			}
			set
			{

			}
		}

		[DataMember]
		[Column]
		[Display(Name = "End As String")]
		public string EndAsString
		{
			get
			{
				return this.End.ToString("dddd MMMM dd, yyyy h:mm tt | MM/dd/yyyy h:mm tt");
			}
			set
			{

			}
		}

		[DataMember]
		[Column]
		[Display(Name = "SlotId As String")]
		public string SlotIdAsString
		{
			get
			{
				return this.SlotId.ToString();
			}
			set
			{

			}
		}

		#endregion

		public override string ToString()
		{
			return Start.DateTime.ToShortTimeString();
		}
    }

}
