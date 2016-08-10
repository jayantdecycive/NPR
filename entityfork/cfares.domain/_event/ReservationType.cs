using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Web.Script.Serialization;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using cfacore.shared.domain._base;
using cfacore.shared.domain.attributes;
using cfares.domain._event.resevent;
using cfares.domain._event._ticket;
using cfares.domain._event.occ;
using Ninject;
using cfares.domain._event.slot;
using cfares.domain._event.slot.tours;
using cfares.domain._event._ticket.tours;
using cfares.domain._event.resevent.store;
using cfares.domain._event.resevent.tours;
using cfares.domain._event._tickets;

namespace cfares.domain._event
{
    [DataContract]
    [SyncUrl("/api/ReservationType")]
    public partial class ReservationType : DomainObject, IReservationType
    {
        public ReservationType(string pid)
            : base()
        {
            Occurrences = null;
            Description = null;
            Name = null;
            this._Id = pid;
        }

        public ReservationType()
            : base()
        {
            Occurrences = null;
            Description = null;
            Name = null;
        }

        public bool ShowOperatorView() {
            return this.ReservationTypeId == "reception";
        }

        public static IResApplicationConfiguration Configuration { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public bool HasFood()
        {
            return (this.GetKernel().Get<IResEvent>().GetType().Name == "GiveawayEvent");
        }

        public bool ShowSlots()
        {
            return (this.GetKernel().Get<IResEvent>().GetType().Name != "SpeakerEvent");
        }

        [Required]
        [Display(Name = "Reservation Website URL")]
        public string Urls
        {
            get
            {
                if (SiteUrls == null)
                    return string.Empty;

                var urls = SiteUrls.Select(x => x.Url).ToList();
                if (urls.Count == 0)
                    return null;
                return string.Join(", ", urls);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                string[] urls = value.Split(',').Select(x => x.Trim()).ToArray();
                if (SiteUrls == null)
                    SiteUrls = new Collection<ResSiteUrl>();
                foreach (var siteUrl in SiteUrls.Where(x => !urls.Contains(x.Url)).ToList())
                {
                    SiteUrls.Remove(siteUrl);
                }
                foreach (var url in urls)
                {
                    if (!string.IsNullOrEmpty(url)&&SiteUrls.All(x => x.Url.Replace("http://","") != url.Replace("http://","")))
                    {
                        var surl = new ResSiteUrl {Url = url};
                        this.SiteUrls.Add(surl);
                    }
                }
            }
        }

        [ScaffoldColumn(false)]
        public string Url
        {
            get
            {
                if (SiteUrls == null)
                    return null;
                ResSiteUrl firstOrDefault = SiteUrls.FirstOrDefault();
                return firstOrDefault != null ? firstOrDefault.Url : string.Empty;
            }
        }

        public virtual ICollection<ResSiteUrl> SiteUrls { get; set; }

        private IKernel _Kernal=null;
        public virtual IKernel GetKernel(){
            if(_Kernal!=null)
                return _Kernal;
            _Kernal = new StandardKernel();


            /*
             * ====RESEVENT====
             */
            switch (this.ReservationTypeId.ToLower())
            {
                case "daily":
                case "chainwideproduct":
                case "product":
                    _Kernal.Bind<IResEvent>().To<GiveawayEvent>();
                    break;
                case "story":
                    _Kernal.Bind<IResEvent>().To<StoryTourEvent>();
                    break;
                case "team":
                    _Kernal.Bind<IResEvent>().To<TeamTourEvent>();
                    break;
                case "lgstory":
                    _Kernal.Bind<IResEvent>().To<LargeStoryTourEvent>();
                    break;
                case "reception":
                    _Kernal.Bind<IResEvent>().To<SpeakerEvent>();
                    break;
                
                case "influence":
                default:
                    _Kernal.Bind<IResEvent>().To<ResEvent>();
                    break;
            }

            /*
             * ====OCCURRENCE====
             */
            switch (this.ReservationTypeId.ToLower())
            {
                case "daily":
                case "chainwideproduct":
                case "product":
                    _Kernal.Bind<IOccurrence>().To<GiveawayOccurrence>();
                    break;
                case "story":
                case "team":
                case "lgstory":
                    _Kernal.Bind<IOccurrence>().To<Occurrence>();
                    break;
                case "familyinfluence":
                case "influence":
                    _Kernal.Bind<IOccurrence>().To<DateOccurrence>();
                    break;
             
                default:
                    _Kernal.Bind<IOccurrence>().To<Occurrence>();
                    break;
            }

            /*
             * ====SLOT====
             */
            switch (this.ReservationTypeId.ToLower())
            {
                case "daily":
                case "chainwideproduct":
                case "product":
                    _Kernal.Bind<ISlot>().To<GiveawaySlot>();
                    break;
                case "story":
                case "team":
                case "lgstory":
                    _Kernal.Bind<ISlot>().To<TourSlot>();
                    break;
                default:
                    _Kernal.Bind<ISlot>().To<Slot>();
                    break;
            }

            /*
             * ====TICKET====
             */
            switch (this.ReservationTypeId.ToLower())
            {
                case "familyinfluence":
                case "influence":
                    _Kernal.Bind<ITicket>().To<DateTicket>();
                    break;
                case "daily":
                case "product":
                case "chainwideproduct":
                    _Kernal.Bind<ITicket>().To<FoodTicket>();
                    break;
                case "reception":
                    _Kernal.Bind<ITicket>().To<GuestTicket>();
                    break;
                case "story":
                case "team":
                case "lgstory":
                    _Kernal.Bind<ITicket>().To<TourTicket>();
                    break;
                default:
                    _Kernal.Bind<ITicket>().To<Ticket>();
                    break;
            }

            if (Configuration != null)
                _Kernal = Configuration.GetKernel(this,_Kernal);
            return _Kernal;
        }

        string TypeName
        {
            get
            {
                return Type().FullName;
            }
            set
            {
                _Type = System.Type.GetType(value);
            }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, May 08, 2012
        /// </created>        
        public virtual List<ResEvent> EventList{get;set;}


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, May 08, 2012
        /// </created>

        [XmlIgnore()]
        public virtual TicketCollection Tickets
        {
            get;
            set;
        }


        [XmlIgnore()]
        public virtual OccurrenceCollection Occurrences { get; private set; }


        [DataMember, Column, Required]
        public string Description { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Tuesday, May 08, 2012
        /// </created>
        private System.Type _Type = null;                
        public System.Type Type()
        {
             return this._Type; 
        }
        public void Type(System.Type value){
            this._Type = value;
        }
        [DataMember]
        [ScaffoldColumn(false)]
        public string StrType {
            get 
			{
	            return this._Type == null ? string.Empty : this._Type.ToString();
            }
	        set { this._Type = System.Type.GetType(value); }
        }


        [DataMember, Column, Required]
        public string Name { get; set; }


        public override string UriBase()
        {
            return "http://res.chick-fil-a.com/event/reseventtype/"; 
        }

        public override string ToChecksum()
        {
            return Name; 
        }

        [DataMember]
        public string ReservationTypeId
        {
            get { return this._Id; }
            set { this._Id = value; }
        }

        [IgnoreDataMember]
        [XmlIgnore]
        [ScriptIgnore]
        public virtual IEnumerable<ResSiteUrl> ProductionUrls
        {
            get {
                if (SiteUrls == null)
                    return null;
                return SiteUrls.Where(x => x.Url!=null&&!x.Url.Contains("local") && !x.Url.StartsWith("http://temp-event")); }
        }

        public IQueryable<ResEvent> ActiveEvents
        {
            get {
                if (EventList == null)
                    return null;
                var events = EventList.Where(x => x.Status == ResEventStatus.Live || x.Status == ResEventStatus.Hidden && x.SiteStart > DateTime.Now && x.SiteEnd < DateTime.Now);
                if (events == null)
                    return new List<ResEvent>().AsQueryable();
                return events.AsQueryable();             
            }
            
        }

        public bool IncludeCorporateByDefault()
        {
            return ReservationTypeId == "reception";
        }

        public bool IsEventActive
        {
            get {
				return Cache.Resolve( string.Format( "EventType{0}-IsEventActive", ReservationTypeId ), delegate
					{
						if (EventList == null) return false;
						var events = EventList.Where(x => ! x.IsEventEnded && ! x.IsEventUpcoming);
						return events.Any();
					} );
            }            
        }
    }
}
