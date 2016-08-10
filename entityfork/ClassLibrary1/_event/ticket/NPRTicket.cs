
#region Imports

using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using cfacore.shared.domain.attributes;
using cfacore.shared.domain.user;
using cfares.domain._event;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using cfares.domain._event._ticket;
using cfares.domain._event.ticket;

#endregion

namespace npr.domain._event.ticket
{
    [DataContract]
    [SyncUrl("/api/NPRTicket")]
    [ModelId("TicketId")]
    public class NPRTicket : Ticket, INPRTicket, IGroupTicket
    {
	    public NPRTicket() {}
        public NPRTicket(bool specialty = false)
        {
            IsSpecialtyTicket = specialty;
        }

        public override int AllocatedCapacity { get { return GroupSize; } }
		public override bool IsAllocatedCapacityFixed { get { return false;  } }

        [DataMember]
        [Column]
        [Display(Name = "Group Size")]
        public int GroupSize { get; set; }

        [DataMember]
        [Display(Name = "Group Type")]
        [Column]
        public string GroupType { get; set; }

	    [DataMember]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Guests")]
        [Column]
        public string GuestListString{ get; set; }

		[DataMember]
		[Display(Name = "Is Paid")]
		[Column]
		public bool IsPaid { get; set; }

		[DataMember]
		[Display(Name = "Ticket Cost")]
		[Column]
		public decimal? TicketAmount { get; set; }

		[DataMember]
		[Display(Name = "Total Amount")]
		[Column]
		public decimal? TotalAmount { get; set; }

		[DataMember]
		[Display(Name = "Is Auth Successful")]
		[Column]
		public bool? IsAuthSuccessful { get; set; }

		[DataMember]
		[Display(Name = "Auth Response")]
		[Column]
		public string AuthorizationResponse { get; set; }

		[DataMember]
		[Display(Name = "Credit Card Number")]
		[Column]
		public string CCNumber { get; set; }

		[DataMember]
		[Display(Name = "Credit Card Expiration Month")]
		[Column]
		public int? CCExpDateMonth { get; set; }

		[DataMember]
		[Display(Name = "Credit Card Expiration Year")]
		[Column]
		public int? CCExpDateYear { get; set; }

		[DataMember]
		[Display(Name = "Billing Name")]
		[Column]
		public string CCName { get; set; }

		[DataMember]
		[Display(Name = "Credit Card Type")]
		[Column]
		public string CCType { get; set; }

		[DataMember]
		[Display(Name = "Listener to NPR station")]
		[Column]
		public string ListenToNprStation { get; set; }

		[DataMember]
		[Display(Name = "Visitor to npr.org")]
		[Column]
		public bool VisitorOfWebsite { get; set; }

		[DataMember]
		[Display(Name = "Age")]
		[Column]
		public string Age { get; set; }

		[DataMember]
		[Display(Name = "Race")]
		[Column]
		public string Race { get; set; }

		[DataMember]
		[Display(Name = "Topics Of Interest")]
		[Column]
		public string TopicsOfInterest { get; set; }

       // [Column]
      //  public string GuestName { get; set; }

		public string TopicsOfInterest1
		{
			get
			{
				return (this.TopicsOfInterest + ",,,").Split(new string[] { "," }, StringSplitOptions.None)[0];
			}
			set
			{
				this.TopicsOfInterest = String.Format("{0},{1},{2}", value, this.TopicsOfInterest2, this.TopicsOfInterest3);
			}
		}
		public string TopicsOfInterest2
		{
			get
			{
				return (this.TopicsOfInterest + ",,,").Split(new string[] { "," }, StringSplitOptions.None)[1];
			}
			set
			{
				this.TopicsOfInterest = String.Format("{0},{1},{2}", this.TopicsOfInterest1, value, this.TopicsOfInterest3);
			}
		}
		public string TopicsOfInterest3
		{
			get
			{
				return (this.TopicsOfInterest + ",,,").Split(new string[] { "," }, StringSplitOptions.None)[2];
			}
			set
			{
				this.TopicsOfInterest = String.Format("{0},{1},{2}", this.TopicsOfInterest1, this.TopicsOfInterest2, value);
			}
		}

        public IEnumerable<Name> GuestList
        {
            get
            {
                if (string.IsNullOrEmpty(GuestListString))
                    return null;
                return GuestListString.Split(',').Select(x=>new Name(x));
            }
            set {
	            GuestListString = value == null ? null : 
					string.Join(",", value.Select(x => x.ToString()));
            }
        }

        public IList<string> TicketGuestList
        {
            get 
            {
                if (string.IsNullOrEmpty(GuestListString))
                    return null;
                List<string> guests = new List<string>();
                foreach (var guestname in GuestListString.Split(','))
                {
                    guests.Add(guestname);
                }
                return guests;
            }
            set
            {
                GuestListString = value == null ? null :
                   string.Join(",", value.Select(x => x.ToString()));
            }
        }

		[Column] 
        [DataMember] 
		[ScaffoldColumn(false)]
        public DateTimeOffset SlotStart 
        { 
            get
            {
	            return Slot == null ? DateTimeOffset.MinValue : Slot.Start;
            }
			set
			{
				if( Slot != null )
					Slot.Start = value;
			} 
        }
		
	
        [Column]
        [Display(Name = "First Name")]
		[ScaffoldColumn(false)]
        public string FirstName
        {
            get
            {
                if (Owner == null) return null;
                return Owner.FirstName; }
            set
            {
				if( Owner != null )
					Owner.FirstName = value;
            }
        }

        [Column]
        [Display(Name = "Last Name")]
		[ScaffoldColumn(false)]
        public string LastName
        {
            get { if (Owner == null) return null; return Owner.LastName; }
            set
            {
				if( Owner != null )
		            Owner.LastName = value;
            }
        }

        [Display(Name = "Email")]
        [Column]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?", ErrorMessage = "Please enter a valid email address")]
		[ScaffoldColumn(false)]
        public string Email
        {
            get { if (Owner == null) return null; return Owner.Email; }
            set
            {
				if( Owner != null )
		            Owner.Email = value;
            }
        }

        
        [Display(Name = "Confirm Email")]
		[ScaffoldColumn(false)]
        public string EmailConfirm
        {
            get
            {
                if (Owner == null) return null;
                return Owner.Username;
            }
            set
            {
				if( Owner != null )
		            Owner.Username = value;
            }
        }

        
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
		[ScaffoldColumn(false)]
        public string Phone
        {
            get { if (Owner == null) return null; return Owner.HomePhoneString; }
            set
            {
				if( Owner != null )
		            Owner.HomePhoneString = value;
            }
        }

        [DataMember]
        [DataType("Enum/_TicketContactPreference")]
        [Display(Name = "Preferred Contact Method")]
        public ContactPreference ContactPreference
        {
            get;
            set;
        }

        //Specialty ticket stuff

        public ICollection<string> ContactNotes { get; set; }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notes")]
        public string Notes
        {
            get
            {
                if (ContactNotes == null || ContactNotes.Count == 0)
                    return string.Empty;
                return string.Join(",",ContactNotes.ToArray());
            }
            set {
	            ContactNotes = string.IsNullOrEmpty(value) ? 
					new List<string>() : 
					new List<string>( value.Split(',') );
            }
        }

        [Column]
        [Display(Name = "Confirmation Number")]
        [DataMember]
		[ScaffoldColumn(false)]
        public string ConfirmationNumber
        {
            get {
	            const int confirmationNumberLength = 8;
				if (string.IsNullOrWhiteSpace(CardNumber)) return string.Empty; 
				return CardNumber.Length < confirmationNumberLength ? string.Empty : 
					CardNumber.Substring(0, confirmationNumberLength).ToUpper();
            }
			set {}
        }

        [DataMember]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Date Selections")]
        public string DatesString{ 
            get {
                if (Dates==null||Dates.Count==0) return null;
                string s = string.Join(",", Dates.Where( o => o != null ).Select( o => o != null ? o.Value : new DateTime() )
					.Where(x => x != null).Select(x => x.ToShortDateString()).ToArray());
				return string.IsNullOrWhiteSpace( s ) ? null : s;
            }
            set {
	            Dates = ! string.IsNullOrEmpty(value) ? 
					value.Split(',')
						.Select( o => new DateTime?( DateTime.Parse( o ) ) )
						.Where(x=>x != null).ToList() : new List<DateTime?>();
            }
        }


        [Display(Name = "Date Selections")]
        public IList<DateTime?> Dates
        {
	        get
	        {
		        IList<DateTime?> l = _dates == null ? null : 
					_dates.Where( o => o != DateTime.MinValue ).ToList();
				
		        while ( l != null && l.Count < 3 ) l.Add( null );

		        return l;
	        }
	        set { _dates = value; }
        }
	    private IList<DateTime?> _dates;

	    [Column]
        [DataMember]
        public bool IsSpecialtyTicket // Now client renamed to "personalized" in UI elements
	    {
		    get { return ! string.IsNullOrWhiteSpace( DatesString ) || _isSpecialtyTicket; }
		    set { _isSpecialtyTicket = value; }
	    }
	    private bool _isSpecialtyTicket;

        public IEnumerable<Name> AllGuestNames
        {
            get { return GuestList; }
          
        }

        public string GroupName
        {
            get { return OwnerName; }
        }
       
      //  public int? TicketGuestsId { get; set; }

        public string QRCodeImageUrl
        {
            get { return String.Format("http://chart.apis.google.com/chart?cht=qr&chs=300x300&chld=L&choe=UTF-8&chl={0}", CardNumber); }
        }
    }

    public enum TourGroupTypes
    {
        [Description("Journalists")]
        Journalists,
        [Description("College/University Students")]
        StudentCollege,
        [Description("High School Students")]
        StudentHighschool,
        [Description("Inters")]
        Interns,
        [Description("NPR Staff")]
        NprStaff,
        [Description("NPR Member Station Staff")]
        NprMemberStationStaff,
        [Description("Public Radio Staff")]
        PublicRadioStaff,
        [Description("Congressional Staff")]
        CongressionalStaff,
        [Description("Foreign Government Officials")]
        ForeignGovernmentOfficials,
        [Description("Trade/Advocacy Organazation Representatives")]
        TradeOrganazationRepresentatives,
        [Description("Librarians")]
        Librarians,
        [Description("Auction Winner")]
        AuctionWinner,
        [Description("Sponsors")]
        Sponsors,
        [Description("Other")]
        Other
    }
}
