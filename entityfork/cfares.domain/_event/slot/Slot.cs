using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using ServiceStack.CacheAccess;
using cfacore.domain._base;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.modules.helpers;
using cfares.domain._event.slot;
using cfacore.shared.domain.attributes;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using cfacore.shared.domain.common;
using cfares.domain._event.slot.tours;

namespace cfares.domain._event
{
    [Table]
    [SyncUrl("/api/Slot")]
    [ModelId("SlotId")]
    [DataContract]
    [Serializable]
    public class Slot : DomainObject, ISlot, IAdminReference,IResEventBound
    {
        public IResEvent GetEvent()
        {
			if (this.Occurrence == null) return null;

            return this.Occurrence.ResEvent;
        }

        public bool HasEvent()
        {
            return this.Occurrence != null && this.Occurrence.ResEventId != null;
        }

        [DataMember]
        [Display(Name = "Apply Schedule Automatically")]
        public bool IsScheduled { get; set; }

	    private IList<Ticket> _tickets; 
	    public virtual IList<Ticket> Tickets 
		{ 
			get
			{
				return _tickets;
			} 
			set
			{
				_tickets = value;
			}

			//get
			//{
			//	string key = string.Format( "Event{0}-Occurrence{1}-Slot{2}-Tickets", Occurrence.ResEventId, OccurrenceId, SlotId );
			//	var ticketsCached = Cache.Get<IList<Ticket>>( key );
			//	if( _tickets == null ) _tickets = ticketsCached;
			//	else if( ticketsCached.Count != _tickets.Count ) Cache.Set( key, _tickets );
			//	return _tickets;
			//}
			//set
			//{
			//	_tickets = value;
			//	string key = string.Format( "Event{0}-Occurrence{1}-Slot{2}-Tickets", Occurrence.ResEventId, OccurrenceId, SlotId );
			//	if( value != null ) Cache.Set( key, value );
			//}
	    }

	    [IgnoreDataMember]
        public virtual IEnumerable<Ticket> IssuedTickets {
            get
            {
				if (Tickets == null) return null;
                return Tickets.Where(x => x.Status != TicketStatus.Partial && 
					x.Status != TicketStatus.Canceled );
            }
            set { }
        }

        /// <summary> 
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Thursday, March 08, 2012
        /// </created>

        [Column]
        [DataType("Enum/_SlotStatus")]
        [ClientDefault("0")]
        [DataMember]
        public SlotStatus Status
        {
            get;
            set;
        }

        [Column]
        [DataMember]
        public int CapacityScheduled {
            //returns slot capacity if it is lower or schedule is null
            //otherwise returns schedule's capacity
            get { 
                return ScheduleId==null?Capacity:(Schedule.Capacity<Capacity?Schedule.Capacity:Capacity);
            }
            set { }
        }

        [Range(0, 1200)]
        [ClientDefault(60)]
        [Column]
        [DataMember]
        public int Capacity
        {
            get { return _capacity; }
            set { _capacity = value; }
        }
        private int _capacity = 60;

	    [Column, DataMember]
	    public int OriginalCapacity
	    {
		    get
		    {
				if( _originalCapacity == default( int ) ) _originalCapacity = Capacity;
			    return _originalCapacity;
		    }
		    set { _originalCapacity = value; }
	    }
	    private int _originalCapacity;

	    [Column]
        [DataMember]
        [DataType("Enum/_ResSlotVisibility")]
		[Display( Name = "Slot Visibility" )]
	    public SlotVisibility Visibility { get; set; }

        [Column]
        [DataMember]
		//[UIHint("type/_TimeZonedDate")]
		[Display(Name = "Slot Start")]
        [DataType("jqui/_DateTimePicker")]
        public DateTimeOffset Start { get; set; }

        [Column]
        [DataMember]
        [DataType("jqui/_DatePicker")]
        public DateTimeOffset Cutoff { get; set; }

        [Column]
        [DataMember]
		[Display(Name = "Cutoff")]
        [DataType("jqui/_DateTimePickerStr")]
        public string CutoffString
        {
            get { return Cutoff.ToUtcString(); }
            set { Cutoff = value.ToDateTimeOffset(); }
        }

        [Column]
        [DataMember]
		//[UIHint("type/_TimeZonedDate")]
		[Display(Name = "Slot End")]
        [DataType("jqui/_DateTimePicker")]
        public DateTimeOffset End { get; set; }

		[Display(Name = "Slot Start")]
        [DataType("jqui/_DateTimePickerStr")]
        public string StartOffsetString
        {
            get { return Start.ToUtcString(); }
            set { Start = value.ToDateTimeOffset(); }
        }

		[Display(Name = "Slot End")]
        [DataType("jqui/_DateTimePickerStr")]
        public string EndOffsetString
        {
            get { return End.ToUtcString(); }
            set { End = value.ToDateTimeOffset(); }
        }

		[Display(Name = "Slot Start Date")]
        [DataType("jqui/_DatePickerStr")]
        public string StartOffsetDateString
        {
            get { return Start.Date.ToString("MM/dd/yyyy"); }
            set { Start = DateTime.Parse(value).Add( new TimeSpan( Start.Ticks ) ); }
        }

	    [Display(Name = "Slot End Date")]
        [DataType("jqui/_DatePickerStr")]
        public string EndOffsetDateString
        {
            get { return End.Date.ToString("MM/dd/yyyy"); }
            set { End = DateTime.Parse(value).Add( new TimeSpan( End.Ticks ) ); }
        }

        [Display(Name = "Slot Start Time")]
        public string StartDisplayString
        {
            get { return Start.Date.ToString("MM/dd/yyyy hh:mm"); }
        }

        [Display(Name = "Slot End Time")]
        public string EndDisplayString
        {
            get { return End.Date.ToString("MM/dd/yyyy hh:mm"); }
        }


        [System.Data.Linq.Mapping.Association(Name = "FK_Occurrence", ThisKey = "OccurrenceId", OtherKey = "OccurrenceId")]
        [UIHint("Badge/_Occurrence")]
        [DataType("Picker/_Occurrence")]
        [DataMember]
        [XmlIgnore]
        [SoapIgnore]
        [ScriptIgnore]
        [ClientDefault(1, type = typeof(string))]
        public virtual Occurrence Occurrence
        {
            get;
            set;
        }

        public string StoreId
        {
            get
            {
                if (Occurrence == null)
                    return null;
                return Occurrence.StoreId;
            }
        }
        
        [DataMember]
        [Column]
        [DataType("Picker/_OccurrenceId")]
        public int? OccurrenceId
        {
            get;
            set;
        }



        [System.Data.Linq.Mapping.Association(Name = "FK_Schedule", ThisKey = "ScheduleId", OtherKey = "ScheduleId")]
        [UIHint("AdminLinkLoadable")]
        [DataType("Picker/_Schedule")]
        [ClientDefault(1, type = typeof(string))]
        public virtual Schedule Schedule
        {
            get;
            set;
        }

        [DataMember]
        [DataType("Picker/_ScheduleId")]
        public int? ScheduleId
        {
            get;
            set;
        }

        public override string ToChecksum()
        {
            return Id() + Start.UtcDateTime.ToShortDateString();
        }


        public Slot() { 
        
        }

        public Slot(Slot parent):base(parent) {
            if (parent != null)
            {
                this.Start = parent.Start;
                this.End = parent.End;
                this.Capacity = parent.Capacity;
                this.Cutoff = parent.Cutoff;
                this.OccurrenceId = parent.OccurrenceId;
                this.Status = parent.Status;
                this.Tickets = null;
                this.ScheduleId = parent.ScheduleId;
                this.IsScheduled = parent.IsScheduled;
            }
        }



        public Slot(string Id)
        {
            this._Id = Id;
        }

        public override string GetEntityType()
        {
            return "Slot";
        }


        public SlotGrouping Grouping { get; set; }
	    
        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }
        
        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/slot/"; 
        }

        protected int _TotalTicketCache = 0;

	    [DataMember]
        public virtual int TicketsAvailable
        {
            get
            {
	            return Capacity - TotalTickets;
            }
            set {
                if (Tickets == null) _TotalTicketCache = Capacity-value;
            }
        }
        [DataMember]
        [Display(Name = "Total Tickets")]
        public virtual int TotalTickets
        {
            get
            {
                if (IssuedTickets == null) return _TotalTicketCache;
                return IssuedTickets.Sum(x=>x.AllocatedCapacity);
            }
            set
            {
                if (IssuedTickets == null)
                    _TotalTicketCache = value;
            }
        }

        [DataMember]
        public int SlotId {
            get { return this.IntId(); }
            set { this.Id(value); }
        }

        public override string ToHtmlString()
        {
            //return string.Format("Slot {2} for <span class='clean-timezone' data-date-format='dddd, MMM d hh:mmtt'>{0}</span> to <span class='clean-timezone' data-date-format='dddd, MMM d hh:mmtt'>{1}</span>", Start.ToString("dddd, MMM d hh:mmtt"), End.ToString("dddd, MMM d hh:mmtt"), SlotId);

            return string.Format( "Slot {2} for <span class='clean-timezone'>" +
				"{0}</span> to <span class='clean-timezone'>{1}</span>", 
				StartOffsetString, 
				EndOffsetString, 
				SlotId);
        }

        public string AnchorLabel()
        {
            string name = this.ToString();
            
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Slot/Details/{0}", Id());
        }

        public virtual void SetOccurrence(IOccurrence occurrence)
        {
            this.Occurrence = occurrence as Occurrence;
        }

        [DataType(DataType.MultilineText)]
	    public string Notes { get; set; }

	    public int CurrentDailyCapacity { get
	    {
			// Return the total capacities added together of all slots for this event for this day
			// .. "this day" is defined by this slot's start datetime
			if( Occurrence == null ) return -1;
			return Occurrence.SlotsList
				.Where( o => o.Start.Date.Equals( Start.Date ) ).Sum( o => o.Capacity );
		} }

	    public int CurrentDailyCapacityExcludingThisSlot { get
	    {
			// Return the total capacities added together of all slots for this event for this day
			// .. "this day" is defined by this slot's start datetime
			if( Occurrence == null ) return -1;
			return Occurrence.SlotsList
				.Where( o => o.SlotId != SlotId && o.Start.Date.Equals( Start.Date ) ).Sum( o => o.Capacity );
		} }

		public bool IsMinimumDailyCapacityMet { get
	    {
			if( Occurrence == null ) return false;
			return CurrentDailyCapacity >= Occurrence.ResEvent.MinimumDailyCapacity;
	    } }

		public bool IsMinimumDailyCapacityMetExcludingThisSlot { get
	    {
			if( Occurrence == null ) return false;
			return CurrentDailyCapacityExcludingThisSlot >= Occurrence.ResEvent.MinimumDailyCapacity;
	    } }

	    public void SetAdditionalData(Dictionary<string, object> data) {}

        public bool AreTicketsAvailable(int? except=0)
        {
            return GetTicketsAvailableExcept(except.Value)>0;
        }

        public override string ToString()
        {
            return string.Format("{1} {0}", StartOffsetString,this.Occurrence.ToString());
        }



        public void SetOffset(TimeZoneInfo timeZoneInfo)
        {
            this.Start = new DateTimeOffset(this.Start.DateTime,timeZoneInfo.GetUtcOffset(this.Start));
            this.End = new DateTimeOffset(this.End.DateTime, timeZoneInfo.GetUtcOffset(this.End));
            this.Cutoff = new DateTimeOffset(this.Cutoff.DateTime, timeZoneInfo.GetUtcOffset(this.Cutoff));
        }

        public int GetTicketsAvailableExcept(int currentTicketId)
        {
            var capacity = IsScheduled ? Schedule.Capacity : Capacity;
            return capacity - (IssuedTickets.Where(o => o.TicketId != currentTicketId).Sum(tt => tt.AllocatedCapacity));
                
        }

        public void InvalidateCache()
        {
            //TODO: invalidate the cache


            Console.WriteLine(String.Format("Cache cleared for slot {0}", this.SlotId));
        }

    }
}
