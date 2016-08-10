
#region Imports

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Ninject;
using cfacore.domain._base;
using cfacore.shared.domain._base;
using cfacore.shared.domain.attributes;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using cfacore.shared.domain.common;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using cfacore.shared.modules.helpers;
using cfares.domain._event._ticket;
using cfares.domain.store;

#endregion

#region Pragmas
// ReSharper disable CheckNamespace
#endregion

namespace cfares.domain._event
{
    public enum OccurrenceStatus
    {
        Locked,
        Live,
        Hidden,
        ProtoType,
        Deactivated
    }

    [Table]
    [SyncUrl("/api/Occurrence")]
    [ModelId("OccurrenceId")]
    [Serializable]
    [DataContract]
    public class Occurrence: DomainObject, IOccurrence, IAdminReference,IResEventBound
    {
        public IResEvent GetEvent()
        {
            return ResEvent;
        }

        public string GetFormattedDateRange(bool dayOfWeek=false,bool fromOn=false)
        {
            return GetFormattedDateRange(GetDates(), dayOfWeek, fromOn);
        }

        public static string GetFormattedDateRange(IList<DateTime> dates, bool dayOfWeek=false,bool fromOn=false)
        {
            string result = string.Empty;
            if (dates.Count() == 1)
            {
                result= dates.First().ToString((dayOfWeek?"dddd, ":"")+"MMMM d, yyyy");
            }
            else if (dates.Count() > 1)
            {
                var startDate = dates.First();
                var endDate = dates.Last();
                if (startDate.Year == endDate.Year && startDate.Month == endDate.Month)
                {
                    result = startDate.ToString("MMMM d") + " - " + endDate.ToString("d, yyyy");
                }
                else if (startDate.Year == endDate.Year)
                {
                    result = startDate.ToString("MMMM d") + " - " + endDate.ToString("MMMM d, yyyy");
                }
                else
                {
                    result = startDate.ToString("MMMM d, yyyy") + " - " + endDate.ToString("MMMM d, yyyy");
                }
            }
            return fromOn?((dates.Count()==1?"On ":"From ")+result):result;
        }

        public bool HasEvent()
        {
            return ResEventId != null;
        }

        public Occurrence() { 
        
        }

        public IList<DateTime> GetDates()
        {
            return SlotsList.GroupBy(x => x.Start.Date).OrderBy(x => x.Key).Select(x => x.Key).ToList();
        }

        [Column]
        [DataType("Enum/_OccurrenceStatus")]
        [DataMember]
        public virtual OccurrenceStatus Status
        {
            get;
            set;
        }

        public Occurrence(string id)
        {
            _Id = id;
        }

        public Occurrence(ResEvent resEvent, ResStore store)
        {
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
            ResEvent = resEvent;
            Store = store;
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        [System.Data.Linq.Mapping.Association(Name = "FK_ResEvent", ThisKey = "ResEventId", OtherKey = "ResEventId")]
        [DataType("Picker/_ResEvent")]
        [UIHint("Badge/_ResEvent")]
        [DataMember]
        [ClientDefault(1, type = typeof(string))]
        public virtual ResEvent ResEvent
        {
            get;
            set;
        }
        [Column]
        [DataMember]
        [DataType("Picker/_ResEventId")]
        public int? ResEventId
        {
            get;
            set;
        }

		/*
		[DataMember]
		[Display(Name = "Start Day")]
		[Column(Expression = "DATENAME(dw, [Start])")]
		public string StartDay 
		{ 
			get; 
			set; 
		}
		*/

        [DataMember]
        public string ResEventName
        {
            get
            {
                if (ResEvent == null)
                    return null;
                return ResEvent.Name;
            }
            set
            {
                if(ResEvent!=null)
                ResEvent.Name = value;
            }
        }


	    /// <summary>
	    /// Generated code from the Core Model Builder Add-on.
	    /// </summary>
	    /// <created>
	    /// Generated Tuesday, March 13, 2012
	    /// </created>


	    [UIHint("RelationalTable/_Slots")]
	    [DataType("Table/_Slots")]
	    [ScriptIgnore]
	    [XmlIgnore]
	    public virtual IList<Slot> SlotsList
	    {
			get; set;
			//get
			//{
			//	return _SlotsList;
			//} 
			//set
			//{
			//	_SlotsList = value;
			//}
			//get
			//{
			//	string key = string.Format( "Event{0}-Occurrence{1}-SlotsList", ResEventId, OccurrenceId );
			//	return Cache.Get<IList<Slot>>( key );
			//}
			//set
			//{
			//	string key = string.Format( "Event{0}-Occurrence{1}-SlotsList", ResEventId, OccurrenceId );
			//	if( value != null ) Cache.Set( key, value );
			//}

			//get
			//{
			//	if( _slotsList == null )
			//	{
			//		string key = string.Format( "Event{0}-Occurrence{1}-SlotsList", ResEventId, OccurrenceId );
			//		using( ICacheClient c = Cache )
			//			_slotsList = c.Get<IList<Slot>>( key );
			//	}
			//	return _slotsList;
			//}
			//set
			//{
			//	_slotsList = value;
			//	string key = string.Format( "Event{0}-Occurrence{1}-SlotsList", ResEventId, OccurrenceId );
			//	if( value != null ) 
			//		using( ICacheClient c = Cache )
			//			Cache.Set( key, value );
			//}
	    }
	    //private IList<Slot> _slotsList; 

	    [ScriptIgnore]
        [XmlIgnore]
        public virtual IQueryable<Slot> AvailableSlots
        {
            get
            {
                if(SlotsList==null)
                    return null;
                var slots = SlotsList.AsQueryable().Include("Tickets").Where(x => 
                    x.Cutoff>DateTime.Now&&
                    x.Tickets.Where(t => t.TicketId != 0 && t.Status != TicketStatus.Partial
						&& t.Status != TicketStatus.Canceled).Sum(t=>t.AllocatedCapacity) < x.Capacity);

                return slots;
            }
        }

        [Column]
        [DataMember]
        [DataType("jqui/_DatePicker")]
        public DateTime Start
        {
            get { return RegistrationAvailability.Start; }
            set { RegistrationAvailability.Start = value; }

        }

		public string StartString
        {
            get { return Start.ToString(CultureInfo.InvariantCulture); }
            set { Start = DateTime.Parse(value); }
        }

		public string StartStringAsDate
		{	            
			get
			{
				if( Start == SqlDateTime.MinValue || Start == DateTime.MinValue ) return "N/A";
				return Start.ToDateString();
			}
		}

		//[Column]
		//[DataMember]
		//[Display(Name = "Start Day")]
		//public string StartDay
		//{
		//	get
		//	{
		//		if (ResEvent == null) return string.Empty;
		//		return Start.DayOfWeek.ToString();
		//	}
		//}

		public DateTimeOffset FirstSlotStart
		{
            get
            {
				if( SlotsList == null ) return DateTime.MinValue;
				Slot slot = SlotsList
					.Where( o => o.Status == SlotStatus.Active || o.Status == SlotStatus.Hidden )
					.OrderBy( o => o.Start ).FirstOrDefault();
				return slot == null ? DateTime.MinValue : slot.Start;
            }
		}

		public string FirstSlotStartStringAsDate
		{
			get
			{
				return FirstSlotStart == DateTime.MinValue ? "N/A" : FirstSlotStart.ToDateString();
			}
		}

		public string FirstSlotStartStringAsTime
		{
			get
			{
				return FirstSlotStart == DateTime.MinValue ? "N/A" : FirstSlotStart.ToTimeString();
			}
		}

		public string StartStringAsTime
		{
			get
			{
				if( Start == SqlDateTime.MinValue || Start == DateTime.MinValue ) return "N/A";
				return Start.ToTimeString();
			}
		}

        [Column]
        [DataType("jqui/_DatePicker")]
        [DataMember]
        public DateTime End
        {
            get { return RegistrationAvailability.End; }
            set { RegistrationAvailability.End = value; }

        }

        public string EndString
        {
            get { return End.ToString(CultureInfo.InvariantCulture); }
            set { End = DateTime.Parse(value); }
        }

        [Column]
        [Display(Name = "Slot Range Start")]
        [DataType("jqui/_DatePicker")]
        public DateTime SlotRangeStart
        {
            get { return SlotRange.Start; }
            set { SlotRange.Start = value; }
        }

        public string SlotRangeStartString
        {
            get { return SlotRangeStart.ToString(CultureInfo.InvariantCulture); }
            set { SlotRangeStart = DateTime.Parse(value); }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, March 13, 2012
        /// </created>		
        [Column]
        [Display(Name = "Slot Range End")]
        [DataType("jqui/_DatePicker")]
        [DataMember]
        public DateTime SlotRangeEnd
        {
            get { return SlotRange.End; }
            set { SlotRange.End = value; }
        }

        public string SlotRangeEndString
        {
            get { return SlotRangeEnd.ToString(CultureInfo.InvariantCulture); }
            set { SlotRangeEnd = DateTime.Parse(value); }
        }

		public bool FullyBooked { get { return TicketsAvailable() <= 0; } }

	    public int Capacity
	    {
		    get
		    {
			    // Special case for performance - If the AllocatedCapacity per ticket is known to be a fixed # ( say "1" ) then instead 
			    // .. of enumerating the Tickets collection, leverage the data-tier and data-tier caching to perform the calculation
			    if ( ResEvent != null && ResEvent.EnablePerformanceOptimizations)
			    {
				    string sql = @"select isnull(sum(s.Capacity), 0) as TotalCapacity
					from Occurrences o join Slots s on s.OccurrenceId = o.OccurrenceId
					where GETDATE() < s.Cutoff
						and o.OccurrenceId = " + OccurrenceId;

				    DbContext c = ReservationType.Configuration.DbContext;
				    ObjectContext oc = (c as IObjectContextAdapter).ObjectContext;
				    int totalTicketsAvailable = oc.ExecuteStoreQuery<int>(sql).First();
				    return totalTicketsAvailable;
			    }

			    // Slots that haven't expired .. tally ( total capacity - total number of tickets reserved )
			    if (SlotsList == null) return 0;
			    DateTime now = DateTime.Now;
			    int query = SlotsList.Where(s => now < s.Cutoff).Sum( s => s.Capacity );
			    return query;
		    }
	    }

	    public int TotalTickets { get
        {
			// Special case for performance - If the AllocatedCapacity per ticket is known to be a fixed # ( say "1" ) then instead 
			// .. of enumerating the Tickets collection, leverage the data-tier and data-tier caching to perform the calculation
			if( ResEvent != null && ResEvent.EnablePerformanceOptimizations )
			{
				string sql = @"select isnull(sum(t.AllocatedCapacity), 0) as TotalTickets
					from Occurrences o
						join Slots s on s.OccurrenceId = o.OccurrenceId
						join Tickets t on t.SlotId = s.SlotId
					where ( t.TicketId is null or t.status = 2 )
						and ( t.TicketId is null or t.TicketId > 0 )
						and GETDATE() < s.Cutoff
						and o.OccurrenceId = " + OccurrenceId;

				DbContext c = ReservationType.Configuration.DbContext;
				ObjectContext oc = (c as IObjectContextAdapter).ObjectContext;
				int totalTickets = oc.ExecuteStoreQuery<int>( sql ).First();
				return totalTickets;
			}

			// Slots that haven't expired .. tally ( total capacity - total number of tickets reserved )
            if (SlotsList == null) return 0;
			DateTime now = DateTime.Now;
            var query = SlotsList.Where(s => now < s.Cutoff)
                .Sum( s => s.Tickets.Where( x => x.TicketId != 0 && x.Status == TicketStatus.Reserved )
						.Sum(t=>t.AllocatedCapacity)
				);
			return query;
        } }

		public int TicketsAvailable() {
			return Cache.Resolve( string.Format( "Occurrence{0}-TicketsAvailable", OccurrenceId ), 
				() => Capacity - TotalTickets, CacheProfile.NearRealtime );
		}

        public int TicketsAvailable(IDateRange when) { throw new NotImplementedException(); }
        public int TicketsAvailable(DateTime when) { throw new NotImplementedException(); }

        public bool AreTicketsAvailable() { return TicketsAvailable() > 0; }
        public bool AreTicketsAvailable(IDateRange when)
        {
            var start = when.Start;
            var end = when.End;
            return AvailableSlots.Where(x => x.Start> start&&x.End<end).Sum(x => x.Capacity - x.IssuedTickets.Sum(t=>t.AllocatedCapacity)) > 0;
        }
        public bool AreTicketsAvailable(DateTime when)
        {
            return AvailableSlots.Where(x=>x.Start.DateTime==when).Sum(x => x.Capacity - x.IssuedTickets.Sum(t=>t.AllocatedCapacity)) > 0;
        }

        public override string ToString()
        {
            return string.Format("{1} at {0}",Store.Name,ResEventName);
        }

        public static Occurrence Parse(string item) {
            Occurrence oc = new Occurrence();
            return oc;
        }


        [Column]
        [Required]
        [DataMember]
        public bool BoundToPrototype
        {
            get;
            set;
        }


        [Column]
        [DataType("Picker/_LocationId")]
        [DataMember]
        public string StoreId
        {
            get;
            set;
        }

        [DataMember]
        public string StoreName
        {
            get
            {
                if (Store == null)
                    return null;
                return Store.Name;
            }
            set
            {
                if (Store != null)
                    Store.Name = value;
            }
        }

        public string AnchorLabel()
        {
            string name = "View Occurrence";
            if (Store.IsBound())
                name = string.Format("Occurrence For Store #{0}", Store.Id());
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Occurrence/Details/{0}", Id());
        }

		public string StartDateAsString { get
		{
		    if (SlotsList == null)
		        return null;
			Slot firstOrDefault = SlotsList.OrderBy(x => x.Start).FirstOrDefault();
			if (firstOrDefault != null)
			{
				DateTimeOffset start = firstOrDefault.Start;


                return start.DateTime.USEnglishDateWithSuffix();
			}
			return string.Empty;
		} }

	    public static TimeSpan ApplyDaylightSavings(string id)
        {

            TimeZoneInfo baseZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            if (!baseZone.IsDaylightSavingTime(DateTime.Now))
            {
                return baseZone.BaseUtcOffset;
            }
		    try
		    {
			    return TimeZoneInfo.FindSystemTimeZoneById(baseZone.DaylightName).BaseUtcOffset;
		    }
		    catch (TimeZoneNotFoundException ex)
		    {
			    Console.Write(ex.Message);
			    return baseZone.BaseUtcOffset.Add(new TimeSpan(0, 1, 0, 0, 0));
			    //return baseZone.BaseUtcOffset.Add(new TimeSpan(0,0,0,0,0));
		    }
        }

        [ClientIgnore]
        public TimeZoneInfo GMTOffset
        {
            get { return _GMTOffset ?? (_GMTOffset = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")); }
	        set
            {
                _GMTOffset = value;
            }
        }

        private TimeZoneInfo _GMTOffset;
        public TimeZoneInfo TimeZoneContext()
        {
            return GMTOffset;
        }
        public static TimeZoneInfo TimeZoneContext(Occurrence Occurrence)
        {
            if (Occurrence == null)
                Occurrence = new Occurrence();
            if (Occurrence.Store != null && Occurrence.Store.IsBound())
                Occurrence.GMTOffset = TimeZoneInfo.FindSystemTimeZoneById(Occurrence.Store.GMTOffset);
            return Occurrence.GMTOffset;
        }
        public static DateTime ConvertToTimeZoneContext(Occurrence Occurrence, DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext(Occurrence);
            return Convert.Add(tzi.GetUtcOffset(Convert));
        }

        public static DateTime ConvertToTimeZoneContext(DateTime Convert)
        {
            Occurrence oc = new Occurrence();
            TimeZoneInfo tzi = oc.TimeZoneContext();
            return Convert.Add(tzi.GetUtcOffset(Convert));
        }


        public static DateRange ConvertToTimeZoneContext(DateRange Convert)
        {
            Occurrence oc = new Occurrence();
            TimeZoneInfo tzi = oc.TimeZoneContext();
            return Convert.Add(tzi.GetUtcOffset(Convert.Start));

        }

        public static DateTime ConvertFromTimeZoneContext(Occurrence Occurrence, DateTime Convert)
        {
            TimeZoneInfo tzi = TimeZoneContext(Occurrence);
            return Convert.Add(-tzi.GetUtcOffset(Convert));
        }

        public static DateTime ConvertFromTimeZoneContext(DateTime Convert)
        {
            Occurrence oc = new Occurrence();
            TimeZoneInfo tzi = oc.TimeZoneContext();
            return Convert.Add(-tzi.GetUtcOffset(Convert));
        }


        public static DateRange ConvertFromTimeZoneContext(DateRange Convert)
        {
            Occurrence oc = new Occurrence();
            TimeZoneInfo tzi = oc.TimeZoneContext();
            return Convert.Add(-tzi.GetUtcOffset(Convert.Start));

        }

        public virtual ISlot CreateSlot()
        {
            var slot = ResEvent.GetKernel().Get<ISlot>();
            
            slot.OccurrenceId=OccurrenceId;
            return slot;
        }

        [Column]
        [DataMember]
        public int OccurrenceId
        {
            get { return IntId(); }
            set { Id(value); }
        }

        public override string ToChecksum()
        {
            return Id();// +Status.GetHashCode();
        }

        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/occurrence/";
        }







        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, March 13, 2012
        /// </created>
        private readonly TicketCollection _Tickets = null;
        [UIHint("Tables/_Tickets")]
        [DataType("RelationalTable/_Tickets")]
        public TicketCollection Tickets
        {
            get { return _Tickets; }

        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, March 13, 2012
        /// </created>        
        [System.Data.Linq.Mapping.Association(Name = "FK_Store", ThisKey = "StoreId", OtherKey = "StoreId")]
        [DataType("AutoComplete/_Store")]
        [UIHint("Badge/_Store")]
        [DataMember]
        public virtual ResStore Store
        {
            get;
            set;
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, March 13, 2012
        /// </created>
        private DateRange _RegistrationAvailability = new DateRange();
        [Column]
        [DataType("jqui/_DateRange")]
        [UIHint("type/_DateRange")]
        public DateRange RegistrationAvailability
        {
            get { return _RegistrationAvailability ?? (_RegistrationAvailability = new DateRange()); }

	        set
            {
                if (_RegistrationAvailability == null)
                    _RegistrationAvailability = new DateRange();
                _RegistrationAvailability = value;
            }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, March 13, 2012
        /// </created>
        private DateRange _SlotRange = new DateRange();

	    [Column]
        [DataType("jqui/_DateRange")]
        public DateRange SlotRange
        {
            get { return _SlotRange; }

            set { _SlotRange = value; }
        }


        public static TimeZoneInfo DefaultTimeZone {
            get
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
        }

        public void InvalidateCache()
        {
            foreach (ISlot slot in SlotsList){
                slot.InvalidateCache();
            }
            //TODO: invalidate the cache


            Console.WriteLine( "Chache has been cleard for occurrence {0}", OccurrenceId );
        }
    }
}

#region Pragmas
// ReSharper restore CheckNamespace
#endregion