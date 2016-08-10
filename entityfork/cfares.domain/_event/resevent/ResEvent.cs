
#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using Ninject;
using cfacore.shared.domain._base;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.media;
using cfacore.shared.modules.helpers;
using cfares.domain._event.resevent;
using cfacore.shared.domain.common;
using cfares.domain._event.slot;
using System.Data.Linq.Mapping;
using cfares.domain.store;
using System.Runtime.Serialization;
using System.Web;

#endregion

#region Pragmas

// ReSharper disable CheckNamespace

#endregion

namespace cfares.domain._event
{
    [Table]
    [SyncUrl("/api/Event")]
    [Serializable]
    public class ResEvent : ResEventMeta, IResEvent, IAdminReference
    {
        public ResEvent()
        {
		    Description = null;
            AutomaticallyEnableOccurrences = false;
            //mdrake:I want to avoid this
        }

	    public ResEvent(string id) : this()
	    {
		    _Id = id;
	    }

	    public IKernel GetKernel() {
            return ReservationType.GetKernel();
        }

        public virtual IList<Occurrence> Occurrences { get; set; }

		public IList<Occurrence> OccurencesDistinctByDate { get {
			if( Occurrences == null ) return new List<Occurrence>();
			return Occurrences.OrderBy( o => o.Start ).GroupBy( o => o.Start.Date, o => o, 
				(key, g) => new { OccurenceStartDate = key, Occurence = g.ToList().First() } )
				.Select( o => o.Occurence ).ToList();
		} }

        private SlotCollection _slotCache;
        public SlotCollection Slots {
            get {
                if (_slotCache == null)
                    if (Occurrences == null)
                        return null;
                    else
                        _slotCache = new SlotCollection(Occurrences.SelectMany(x=>x.SlotsList).ToList());
                return _slotCache;
            }
            set {
                _slotCache = value;
            }
        }

		private DateRange _registrationAvailability = new DateRange(DateTime.Now, DateTime.Now);
        
        public DateRange GetRegistrationAvailability()
        {
	        return _registrationAvailability ?? (_registrationAvailability = new DateRange());
        }

        [UIHint("type/_DateRange")]
        public DateRange RegistrationAvailability {
            get
            {
                return GetRegistrationAvailability();
            }
        }

	    public void SetRegistrationAvailability(DateRange value)
        {
                if (_registrationAvailability == null)
                    _registrationAvailability = new DateRange(); 
                _registrationAvailability = value;
        }

	    public bool IsEventEnded
	    {
		    get
		    {
			    // Use UTC timestamp values to determine if event has expired
				// .. TODO - Ensure values stored as UTC to avoid DST continuity / other issues
			    DateTime n = DateTime.Now;
				return n > RegistrationEnd || n > SiteEnd;
		    }
	    }

        public bool IsEventUpcoming
        {
            get
            {
                // Use UTC timestamp values to determine if event has expired
                // .. TODO - Ensure values stored as UTC to avoid DST continuity / other issues
                DateTime n = DateTime.Now;
                return n < SiteStart;
            }
        }

        [Display(Name = "Make this a \"Featured\" Event")]
	    public bool IsFeatured { get; set; }

		[Display(Name = "Make this a \"Paid\" Event")]
		public bool IsPaid { get; set; }

		[Display(Name = "Ticket Amount")]
		public decimal? TicketAmount { get; set; }

	    public string CutoffString { get
	    {
	        if (Occurrences == null) return string.Empty;
			IList<Slot> sl = Occurrences.Select( o => o.SlotsList ).FirstOrDefault();
			if( sl == null ) return string.Empty;
			Slot s = sl.OrderBy( o => o.Start ).FirstOrDefault();
			if( s == null ) return string.Empty;

			// E.g. "2:00 PM - Two Days Before the Event"
		    int daysBeforeEvent = Convert.ToInt32( ( s.Start.Date - s.Cutoff.Date ).TotalDays );
			if( daysBeforeEvent > 0 )
				return string.Format( "{0} - {1} Day{2} Before the Event", s.Cutoff.ToTimeStringLong(), 
					daysBeforeEvent > 1 ? daysBeforeEvent.ToWrittenNumber() : string.Empty, 
					daysBeforeEvent > 1 ? "s" : string.Empty );

				return string.Format( "{0} - The Day of the Event", s.Cutoff.ToTimeStringLong() );
		} }

	    public bool FullyBooked { get {
			return Cache.Resolve( string.Format( "Event{0}-FullyBooked", ResEventId ), 
				() => TicketsAvailable <= 0, CacheProfile.UpdatedEveryFewMinutes );
	    } }

	    public int TotalTicketsIssued { get
	    {
	        if (Occurrences == null) return 0;
			return Occurrences
				.SelectMany( o => o.SlotsList )
				.Sum( s => s.TotalTickets );
	    } }

	    public virtual int TotalReservationsRemaining { get
	    {
	        if (Occurrences == null) return 0;
            if ((MaximumCapacity ?? 0) > 0) { return MaximumCapacity.Value - TotalTicketsIssued; }

			return Occurrences
				.SelectMany( o => o.SlotsList )
				.Sum( s => s.Capacity - s.TotalTickets );
	    } }

        public string MediaUrl
        {
            get { return Media == null ? null : Media.MediaUriStr; }
            set { }
        }

        public IEnumerable<Slot> ActiveSlotsByCurrentDateTime { get
	    {
			DateTime now = DateTime.Now;
	        if (Occurrences == null) return new List<Slot>();
		    return Occurrences
			    .Where(o => o.Start < now && now < o.End)
			    .SelectMany(o => o.SlotsList
				    .Where(s => s.Start < now && now < s.End && now < s.Cutoff));
	    } }

	    public class DailyCapacityResult
	    {
		    public DateTime Date { get; set; }
		    public int Capacity { get; set; }
		    public string Key { get; set; }

			public override string ToString()
			{
				return string.Format( "{0} on {1}", Capacity, Date.ToDateString() );
			}
	    }

	    public virtual DailyCapacityResult LowestDailyCapacity { get
	    {
	        if (Occurrences == null) return null;
			DailyCapacityResult r = Occurrences.SelectMany( o => o.SlotsList )
				.GroupBy( k => string.Format( "{0}_{1}", k.Start.Date, k.OccurrenceId ), v => v ).OrderBy( g => g.Sum( o => o.Capacity ) )
				.Select( g => new DailyCapacityResult { Key = g.Key, Date = DateTime.Parse( g.Key.Split( new char[] { '_' } )[0] ), Capacity = g.Sum( o => o.Capacity ) } ).FirstOrDefault();
		    return r;
	    } }

	    public virtual DailyCapacityResult GetLowestDailyCapacityByOccurrenceId( int occurrenceId )
	    {
			return GetLowestDailyCapacityByOccurrenceId( Occurrences.Where( o => o.OccurrenceId == occurrenceId ).SelectMany( o => o.SlotsList ).ToList() );
	    }

	    public virtual DailyCapacityResult GetLowestDailyCapacityByOccurrenceId( IList<Slot> slotList )
	    {
			DailyCapacityResult r = slotList
				.GroupBy( k => string.Format( "{0}_{1}", k.Start.Date, k.OccurrenceId ), v => v ).OrderBy( g => g.Sum( o => o.Capacity ) )
				.Select( g => new DailyCapacityResult { Key = g.Key, 
					Date = DateTime.Parse( g.Key.Split( new[] { '_' } )[0] ), 
					Capacity = g.Sum( o => o.Capacity ) } ).FirstOrDefault();
		    return r;
	    }

	    public void ValidateMinimumDailyCapacity()
	    {
	        DailyCapacityResult lc = LowestDailyCapacity;
			if( lc != null && MinimumDailyCapacity > lc.Capacity )
				throw new ApplicationException( string.Format( "Minimum Daily Capacity is greater than the configured capacity for this event " + 
					"[ Minimum Daily Capacity = {0}, Configured Lowest Daily Capacity = {1}, Date on which " + 
					"Lowest Capacity Occurs = {2} ]", MinimumDailyCapacity, lc.Capacity, lc.Date.ToDateStringWithDayOfWeek() ) );
	    }

	    public virtual int TotalCapacity { get
	    {
	        if (Occurrences == null) return 0;
			return Occurrences
				.SelectMany( o => o.SlotsList )
				.Sum( s => s.Capacity );
	    } }

        public virtual int TotalCapacityByCurrentDateTime
        {
            get
	    {
			// See above [ TotalReservationsRemaining ] field for reference
			DateTime now = DateTime.Now;
	        if (Occurrences == null) return 0;
			return Occurrences
				.Where( o => o.Start < now && now < o.End )
				.SelectMany( o => o.SlotsList
					.Where( s => s.Start < now && now < s.End && now < s.Cutoff ) )
				.Sum( s => s.Capacity );
	    } }

	    public IEnumerable<Slot> AvailableSlots { get 
		{
		    return Occurrences == null ? null : Occurrences.SelectMany( o => o.SlotsList );
	    } }

	    public IEnumerable<Slot> AvailableSlotsByCurrentDateTime { get
	    {
			// Every slot for every occurence that hasn't expired
			DateTime now = DateTime.Now;
	        if (Occurrences == null)
	            return null;
			return Occurrences
				.Where( o => o.Start < now && now < o.End )
				.SelectMany( o => o.SlotsList
					.Where( s => s.Start < now && now < s.End && now < s.Cutoff ) );
	    } }

	    public DateTime? EventStart { get
	    {
	        if( Occurrences == null ) return null;
			Occurrence occurrence = Occurrences.OrderBy( o => o.Start ).FirstOrDefault();
			if( occurrence == null ) return null;
		    return occurrence.Start;
	    } }

        [Column]
        [DataType("jqui/_DatePicker")]
        public DateTime RegistrationStart
        {
            get{
                
                return GetRegistrationAvailability().Start;
            }
            set {               
                    GetRegistrationAvailability().Start = value;                
            }
        }
        
        [Column]
        [DataType("jqui/_DatePicker")]
        public DateTime RegistrationEnd
        {
            get { return GetRegistrationAvailability().End; }
            set
            {                
                    GetRegistrationAvailability().End = value;
            }
        }

        private DateRange _siteAvailability = new DateRange(DateTime.Now, DateTime.Now);

		//[DataType("jqui/_DateRange")]
        //[UIHint("type/_DateRange")]
        //[Display(Name = "Slot Visibility Date")]
        public DateRange GetSiteAvailability()
        {
	        return _siteAvailability ?? (_siteAvailability = new DateRange());
        }

        [UIHint("type/_DateRange")]
        public DateRange SiteAvailability {
            get {
                return GetSiteAvailability();
            }
        }

	    public void SetSiteAvailability(DateRange dt)
        {
            _siteAvailability=dt;
        }
        
        [Column]
        [Required]
        [DataType("jqui/_DatePicker")]
        public DateTime SiteStart
        {
            get{ return GetSiteAvailability().Start;}
            set
            {                
                    GetSiteAvailability().Start = value;
            }

        }

        [Column]
        [Required]
        [DataType("jqui/_DatePickerStr")]
        public string SiteStartString {
            get { return SiteStart.ToString("MM/dd/yyyy"); }
            set { SiteStart = DateTime.Parse(value); }
        }

        [Column]
        [Required]
        [DataType("jqui/_DatePickerStr")]
        public string SiteEndString
        {
            get { return SiteEnd.ToString("MM/dd/yyyy"); }
            set { SiteEnd = DateTime.Parse(value); }
        }

        [Column]
        [Required]
        [DataType("jqui/_DatePickerStr")]
        public string RegistrationStartString
        {
            get { return RegistrationStart.ToString("MM/dd/yyyy"); }
            set { RegistrationStart = DateTime.Parse(value); }
        }

        [Column]
        [Required]
        [DataType("jqui/_DatePickerStr")]
        public string RegistrationEndString
        {
            get { return RegistrationEnd.ToString("MM/dd/yyyy"); }
            set { RegistrationEnd = DateTime.Parse(value); }
        }

        /// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Monday, March 12, 2012
		/// </created>		
        [Column]
        [Required]
        [DataType("jqui/_DatePicker")]
        public DateTime SiteEnd
        {
            get{ return GetSiteAvailability().End;}
            set
            {
                GetSiteAvailability().End = value;
            }
        }

        [Display(Name = "Reservation Website URL")]
        public string UrlsAsHtml {
            get
            {
                var routeurl = "http://" + HttpContext.Current.Request.Url.Authority +"/";
                //var a = HttpContext.Current.Request.Url.AbsoluteUri;
                if (SiteUrls == null) return string.Empty;
                List<string> urls = SiteUrls.Select( x => string.Format(
                    "<a target=\"_blank\" href=\"{0}\">{0}</a>", (routeurl + x.Url))).ToList();
                if (urls.Count == 0) return null;
                return string.Join(", ",urls);
            }
        }

        [Required]
        [Display(Name = "Reservation Website URL")]
        public string Urls {
            get
            {
                if (SiteUrls == null)
                    return string.Empty;

                var urls = SiteUrls.Select(x => x.Url).ToList();
                if (urls.Count == 0)
                    return null;
                return string.Join(", ",urls);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                string[] urls = value.Split(',').Select(x=>x.Trim()).ToArray();
                if(SiteUrls==null)
                    SiteUrls = new Collection<ResSiteUrl>();
                foreach (var siteUrl in SiteUrls.Where(x=>!urls.Contains(x.Url)).ToList())
                {
                    SiteUrls.Remove(siteUrl);
                }
                foreach (var url in urls)
                {
                    if(SiteUrls.All(x => x.Url != url) && !string.IsNullOrEmpty(url))
                        SiteUrls.Add(new ResSiteUrl {Url=url,ResEventId = ResEventId});
                }
            }
        }

      //  public string OriginalUrl { get; set; }

        public string Url
        {
            get
            {
	            ResSiteUrl firstOrDefault = SiteUrls.FirstOrDefault();
	            return firstOrDefault != null ? firstOrDefault.Url : string.Empty;
            }
        }

        public virtual ICollection<ResSiteUrl> SiteUrls { get; set; }

        public virtual IEnumerable<ResSiteUrl> ProductionUrls {
            get { return SiteUrls.Where(x => !x.Url.Contains("local") && !x.Url.StartsWith("http://temp-event")); }
        }

        [Column]
        [Display(Name = "SubHeading", Description = "This is the one line description of the event.")]
        [DataType("Html/_HtmlEditor")]
        public string SubHeading { get; set; }

        [Column]
        [Display(Name = "Description", Description = "In 500 characters or less, describe this event.")]
        [DataType("Html/_HtmlEditorSecond")]
        public string Description { get; set; }

        [Column, Display( Name = "You must be 21 or over to attend this event." )]
		public bool MustBeOfAgeToAttend { get; set; }

		public int LocationCount
		{ 
			get { return Occurrences == null ? 0 : Occurrences.Count; }
			set { }
		}

        [Column]
        [DataType("Picker/_ResType")]
        public virtual ReservationType ReservationType { get; set; }

        [Column]
        [Required]
        [DataType("Picker/_ResType")]
        public string ReservationTypeId { get; set; }

        [Column]
        [DataType("DropDown/_ResCategory")]
		[System.ComponentModel.DataAnnotations.Schema.InverseProperty("ReservationCategoryId")]
		[System.ComponentModel.DataAnnotations.Schema.ForeignKey("CategoryId")]	    
		public virtual ReservationCategory Category { get; set; }

		[Column]
        [DataType("DropDown/_ResCategory")]
	    public int? CategoryId { get; set; }

        [Column]
        [DataType("Picker/_ResTemplate")]
        [Display(Name = "Template")]
        public virtual ResTemplate Template { get; set; }

        [Column]
        [DataType("Picker/_ResTemplate")]
        [Display(Name = "Template Name")]
        public string TemplateId { get; set; }

        [Column]
        [DataType("Picker/_MediaId")]
        [Display(Name = "Image")]
        public int? MediaId { get; set; }

        [Column]
        [Display(Name = "Media")]
        public virtual Media Media { get; set; }

        [IgnoreDataMember]
	    public string OrganizationName { get; set; }

		[IgnoreDataMember]
	    public string OrganizationDisplayName { get; set; }

        public IEnumerable<ResStore> GetParticipatingStores()
        {
            return ParticipatingStoresList;
        }

	    [IgnoreDataMember]
        public virtual IEnumerable<ResStore> ParticipatingStoresList
        {
            get {
                if (Occurrences == null)
                    return new List<ResStore>();
                var q =  Occurrences.Where(x => x.StoreId!=null).Select(x => x.Store);
                return q;
            }
        }

        [IgnoreDataMember]
        public virtual IEnumerable<ResStore> ParticipatingStoresListLive
        {
            get
            {
                if (Occurrences == null)
                    return new List<ResStore>();
                var q = Occurrences.Where(x => (x.Status == OccurrenceStatus.Live) && x.StoreId != null).Select(x => x.Store);
                return q;
            }
        }

        [IgnoreDataMember]
        public virtual IQueryable<Occurrence> ActiveOccurrences
        {
            get
            {
                if (Occurrences == null)
                    return new List<Occurrence>().AsQueryable();
                var q = Occurrences.Where(x => (x.Status == OccurrenceStatus.Live) && x.StoreId != null);
                return q.AsQueryable();
            }
        }

        public virtual IEnumerable<object> ParticipatingStoresListViewModel {
            get
            {
                return ParticipatingStoresList.Select(x =>
                    new
                    {
	                    x.LocationNumber, 
						x.Name, 
						x.MarketableUrlString,
                        ConceptCodeId = (int)x.ConceptCode, 
						x.ConceptCode, 
						x.RegionName, 
						x.PhoneString, 
						x.StreetAddress
                    });
            }
        }

        public bool SlotIsAvailable(DateTime when)
        {
            throw new NotImplementedException();
        }

        public Slot SlotAt(DateTime when)
        {
            throw new NotImplementedException();
        }

	    [Column]
        [DataType("Enum/_ResEventStatus")]
        public ResEventStatus Status { get; set; }

	    private ResEventVisibility _visibility;

		[Column]
        [DataType("Enum/_ResEventVisibility")]
		[Display( Name = "Reservation Visibility" )]
	    public ResEventVisibility Visibility
	    {
		    get { return _visibility; }
		    set
		    {
				// Only perform status change logic if visibility is actually changing
				// .. [ Public = Live, Private = Hidden ]
			    if( _visibility != value )

					// Only update status ( on visibility change ) if live or hidden 
					// .. ( archived, deleted, draft and others stay the same )
					if( Status == ResEventStatus.Live || Status == ResEventStatus.Hidden )
						Status = ( value == ResEventVisibility.Public ? 
							ResEventStatus.Live : ResEventStatus.Hidden );

				_visibility = value;
		    }
	    }

	    [Column]
        public int ResEventId {
            get { return IntId(); }
            set { Id(value); }
        }

	    public string ToSummaryString()
	    {
		    return string.Format(@"
Name: *{0}*
Url: *{1}*
Description: *{2}*
Template: *{3}*
ReservationType: *{4}*
Status: *{5}*
", 
				Name, Urls, Description, Template.ResTemplateId, ReservationType.ReservationTypeId, Status.ToString());
	    }

	    public override string ToString()
        {
            return Name;
        }
        
        public string AnchorLabel()
        {
            string name = "View Event";
            if (IsBound())
                name = Name;
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Event/Details/{0}", Id());
        }

        public bool HasStores() {
            return ReservationType.StrType.Contains("Store") || ReservationType.StrType.Contains("Product");
        }

        
        [Column]
        [UIHint("Badge/_Occurrence")]
        [DataType("Picker/_Occurrence")]
        public virtual IOccurrence ProtoOccurrence
        {
            get {
                if (Occurrences != null)                    
                    return Occurrences.FirstOrDefault(x => x.Status == OccurrenceStatus.ProtoType);
                return null;
            }            
        }

        public bool IsTemp()
        {
            return Status == ResEventStatus.Temp;
        }

        public IOccurrence GetOrCreateOccurrence(ResStore store)
        {
            Occurrence o = Occurrences.FirstOrDefault(x=>x.StoreId==store.LocationNumber);
            if (o==null) {
                o = new Occurrence(this, store);
	            Occurrences.Add(o);
            }
            return o;
        }

        public IOccurrence GetOccurrence(Func<IOccurrence, bool> predicate)
        {
            return Occurrences.FirstOrDefault(predicate);
        }

        
        public IOccurrence CreateOrDefaultOccurrenceByStoreId(string store)
        {
            IOccurrence o = GetOccurrence(x => x.StoreId == store);
            if (o == null)
            {
                o = ReservationType.GetKernel().Get<IOccurrence>();
                o.ResEventId = ResEventId;
                o.StoreId = store;
                o.Start = SiteStart;
                o.End = SiteEnd;
                o.SlotRangeStart = RegistrationStart;
                o.SlotRangeEnd = RegistrationEnd;
            }
            else
            {
                o = null;
            }
            return o;
        }

		public bool EnablePerformanceOptimizations { get {
			return ReservationType.Configuration != null 
				&& ReservationType.Configuration.EnablePerformanceOptimizations
				&& TicketInstance.IsAllocatedCapacityFixed;
		}}

		public Type TicketType { get {
			return ReservationType.Configuration.GetKernel( ReservationType ).Get<ITicket>().GetType();
		}}

		public ITicket TicketInstance { get {
			return ReservationType.Configuration.GetKernel( ReservationType ).Get<ITicket>();
		}}

		public Type SlotType { get {
			return ReservationType.Configuration.GetKernel( ReservationType ).Get<ISlot>().GetType();
		}}

        public int TicketsAvailableByCurrentDateTime { get
	    {
			// Get every slot for every occurence that hasn't expired
			// .. Then tally the total capacity - the total number of tickets reserved
	        if (Occurrences == null) return 0;
		    List<Slot> l = ActiveSlotsByCurrentDateTime.ToList();
            if ((MaximumCapacity ?? 0) > 0) { return MaximumCapacity.Value - l.Sum(s => s.TotalTickets); }
            return l.Sum( s => s.TicketsAvailable );
	    } }

        public int TicketsAvailable { get
	    {
			// Get every slot for every occurence that hasn't expired ( entire registration 
			// period, no restrictions ) .. Then tally the total capacity - the total number of tickets reserved
	        if (Occurrences == null) return 0;

			// if a MaximumCapacity is defined it will use that instead of slot capacity
            if( ( MaximumCapacity ?? 0 ) > 0 )
	            if( MaximumCapacity != null ) return MaximumCapacity.Value - TotalTickets();

			// Special case for performance - If the AllocatedCapacity per ticket is known to be a fixed # ( say "1" ) then instead 
			// .. of enumerating the Tickets collection, leverage the data-tier and data-tier caching to perform the calculation
			if( EnablePerformanceOptimizations )
			{
				string sql = @"select isnull(sum(s.Capacity) - sum(t.AllocatedCapacity), 0) as TotalTicketsAvailable
					from ResEvents e
						join Occurrences o on o.ResEventId = e.ResEventId
						join Slots s on s.OccurrenceId = o.OccurrenceId
						left join Tickets t on t.SlotId = s.SlotId
					where ( t.TicketId is null or t.status = 2 )
						and e.ResEventId = " + ResEventId;

				DbContext c = ReservationType.Configuration.DbContext;
				ObjectContext oc = (c as IObjectContextAdapter).ObjectContext;
				int totalTicketsAvailable = oc.ExecuteStoreQuery<int>( sql ).First();
				return totalTicketsAvailable;
			}

			return Slots.Sum( s => s.TicketsAvailable );
	    } }

        public bool HasTickets() { return TotalTickets() > 0; }
        public bool HasTickets(Func<Ticket, bool> exp) { return TotalTickets(exp) > 0; }

        public int TotalTickets()
        {
			// Special case for performance - If the AllocatedCapacity per ticket is known to be a fixed # ( say "1" ) then instead 
			// .. of enumerating the Tickets collection, leverage the data-tier and data-tier caching to perform the calculation
			if( EnablePerformanceOptimizations )
			{
				string sql = @"select isnull(sum(t.AllocatedCapacity), 0) as TotalTicketsIssued
					from ResEvents e
						join Occurrences o on o.ResEventId = e.ResEventId
						join Slots s on s.OccurrenceId = o.OccurrenceId
						join Tickets t on t.SlotId = s.SlotId
					where t.status = 2
						and e.ResEventId = " + ResEventId;

				DbContext c = ReservationType.Configuration.DbContext;
				ObjectContext oc = (c as IObjectContextAdapter).ObjectContext;
				int totalTicketsIssued = oc.ExecuteStoreQuery<int>( sql ).First();
				return totalTicketsIssued;
			}

	        return Occurrences == null ? 0 : GetTickets().Sum(x=>x.AllocatedCapacity);
        }
	    public int TotalTickets(Func<Ticket,bool> exp) { return Occurrences == null ? 0 : GetTickets().Where(exp).Sum(x=>x.AllocatedCapacity); }

	    public IEnumerable<Ticket> GetTickets()
        {
            return Occurrences.AsQueryable().Include("SlotsList.Tickets").SelectMany(x => x.SlotsList).SelectMany(x=>x.Tickets);
        }
        public IEnumerable<Ticket> GetTickets(Func<Ticket, bool> exp)
        {
            return GetTickets().Where(exp);
        }

        public int? MinOccurrenceCapacity { get; set; }
        public bool AutomaticallyEnableOccurrences { get; set; }

		[Column]
        [Display(Name = "Total Event Capacity", Description = "Takes precedent over slot capacity")]
        public int? MaximumCapacity { get; set; }

        [Column]
        [Display(Name = "Minimum Daily Capacity", Description = "** Optional: Choose a minimum daily capacity for this event.  Be careful the value entered does not exceed the total value of all Reservation Limits for slots defined")]
	    public int MinimumDailyCapacity { get; set; }
    }
}

#region Pragmas

// ReSharper restore CheckNamespace

#endregion
