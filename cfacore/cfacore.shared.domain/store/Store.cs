
#region Imports

using System;
using System.Collections.Generic;
using cfacore.domain.store;
using cfacore.domain._base;
using cfacore.shared.domain.user;
using cfacore.shared.domain.attributes;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using cfacore.domain.user;
using cfacore.shared.domain.common;
using System.Text.RegularExpressions;

#endregion

namespace cfacore.shared.domain.store
{
    [Table(Name = "Location")]
    [SyncUrl("/api/Store")]    
    [ModelId("LocationNumber","string:5")]    
    public partial class Store : DomainObject, IStore,IAdminReference
    {
        public override string UriBase(){
        {
            return "http://data.core.chick-fil-a.com/location/store/"; }
        }

        public Store()
        {
            UnitMarketingDirector = null;
            SiteStatus = null;
            ServiceTeamName = null;
            RegionName = null;
            PriceGroupNumber = null;
            OperatorTeamName = null;
            Name = null;
            LocationDescription = null;
            Message = null;
            MarketableName = null;
            CorporateOwned = false;
        }

        public Store(string ID)
        {
            UnitMarketingDirector = null;
            SiteStatus = null;
            ServiceTeamName = null;
            RegionName = null;
            PriceGroupNumber = null;
            OperatorTeamName = null;
            Name = null;
            LocationDescription = null;
            Message = null;
            MarketableName = null;
            CorporateOwned = false;
            this._Id = ID;
        }


        public override string Id()
        {
            if (LocationNumber == null)
            {
                return "0";
            }
            return LocationNumber.ToString();
        }

        public override void Id(string Id){
            LocationNumber=Id;
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private string _LocationNumber = null;

        [Column]
        [Required]
        [Key]
        public string LocationNumber
        {
            get { return this._LocationNumber; }

            set { this._LocationNumber = value; _Bound = true; }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        

        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "BusinessConsultantId", OtherKey = "UserId")]
        public virtual User BusinessConsultant
        {
            get; set; }

        [DataMember]
        [DataType("AutoComplete/_UserId")]
        public int? BusinessConsultantId
        {
            get;
            set;
        }

        public string MapUrl
        {
            get { 
                return string.Format("http://maps.google.com/maps?q={0},{1}", Lat, Lon); 
            }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        
        [Column]
        [DataType("Enum/_ConceptCode")]
        public ConceptCode ConceptCode
        {
            get; set; 
        }

        

        [Column, Required]
        public bool CorporateOwned { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [System.Data.Linq.Mapping.Association(Name = "FK_Address", ThisKey = "BillingAddressId", OtherKey = "AddressId")]
        public virtual Address BillingAddress
        {
            get;
            set;
        }
        public int? BillingAddressId { get; set; }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>        
        [System.Data.Linq.Mapping.Association(Name = "FK_Address", ThisKey = "ShippingAddressId", OtherKey = "AddressId")]
        public virtual Address ShippingAddress
        {
            get;
            set;
        }
        public int? ShippingAddressId { get; set; }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [System.Data.Linq.Mapping.Association(Name = "FK_Address", ThisKey = "StreetAddressId", OtherKey = "AddressId")]
        public virtual Address StreetAddress
        {
            get;
            set;
        }

        public int? StreetAddressId { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
       
        [Column]
        [Required]
        public Phone Phone
        {
            get; set; }

        public string PhoneString
        {
            get
            {
                if (this.Phone == null)
                    return string.Empty;
                return this.Phone.ToFormattedString();
            }
            set { this.Phone = new Phone(value); }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [Column]
        //[Required]??? TODO: Ask Matt
        public Phone Fax
        {
            get; set; }

        public string FaxString
        {
            get
            {
                if (this.Fax == null)
                    return string.Empty;
                return this.Fax.ToFormattedString();
            }
            set { this.Fax = new Phone(value); }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        

        [Column]        
        public Phone VoiceMail
        {
            get; set; }

        public string VoiceMailString {
            get
            {
                if (this.VoiceMail == null)
                    return string.Empty;
                return this.VoiceMail.ToFormattedString();
            }
            set { this.VoiceMail = new Phone(value); }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        

        [Column]
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Email address")]
        public string Email
        {
            get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>        

        [Column]
        [DataType("DropDown/_Distributor")]
        public Distributor Distributor
        {
            get;
            set;
        }

        [DataType("DropDown/_DistributorId")]
        public string DistributorId { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        
        public StoreFeatures Features
        {
            get
            {
               return new StoreFeatures()
                   {
                       AcceptsCfaCard = AcceptsCfaCard,
                       HasDiningRoom = HasDiningRoom,
                       HasDriveThru = HasDriveThru,
                       OffersOnlineOrdering = OffersOnlineOrdering,
                       OffersWireless = OffersWireless,
                       Playground = Playground,
                       ServesBreakfast = ServesBreakfast
                   };
            }
            
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [Column]
        [Required]
        public bool AcceptsCfaCard { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [Column]
        [Required]
        public bool HasDiningRoom { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>		

        [Column]
        [Required]
        public bool HasDriveThru { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>		

        [Column]
        [Required]
        public bool OffersOnlineOrdering { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>		
        [Column]
        [Required]
        public bool OffersWireless { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>		

        [Column]
        [Required]
        public string Playground
        {
            get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>		
        [Column]
        [Required]
        public bool ServesBreakfast { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>        
        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "FinancialConsultantId", OtherKey = "UserId")]
        [DataType("AutoComplete/_User")]
        [UIHint("Badge/_User")]
        public virtual User FinancialConsultant
        {
            get;
            set;
        }

        [DataMember]
        [DataType("AutoComplete/_UserId")]
        public int? FinancialConsultantId
        {
            get;
            set;
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public string GMTOffset
        {
            get;
            set;
        }

        public int GMTOffsetApplied {
            get {
                if (string.IsNullOrEmpty(GMTOffset))
                    return 0;
                return int.Parse(Regex.Replace(Regex.Replace(GMTOffset.Trim(), @"[^\d\-]", ""),"00$","")); }
            set { GMTOffset = string.Format("GMT {0}:00", value.ToString("D2")); }
        }

        
        public string GMTOffsetTimeZone
        {
            get {
                return GMTOffsetTimeZoneInfo.Id;
            }
            set
            {
                GMTOffsetTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(value);
            }
        }

        public TimeZoneInfo GMTOffsetTimeZoneInfo
        {
            get
            {
                return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneCodeDictionary[GMTOffsetApplied]);
            }
            set
            {
                GMTOffsetApplied = (int)value.BaseUtcOffset.TotalHours;
            }
        }

        public bool DayLightSavings {
            get { return true; }
            set { }
        }

        protected Dictionary<int, string> TimeZoneCodeDictionary {
            get {
                return new Dictionary<int, string>(){
                    {0,"GMT Standard Time"},
                    {-5,"Eastern Standard Time"},
                    {-6,"Central Standard Time"},
                    {-7,"US Mountain Standard Time"},
                    {-8,"Pacific Standard Time"},
                };
            }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        
        public GeographicCoordinate Coordinates
        {
            get { return new GeographicCoordinate(Lat,Lon);}
            set { Lat = value.Latitude;
                Lon = value.Longitude;
            }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public double Lat
        {
            get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public double Lon
        {
            get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        [Column]
        public LocationCode LocationCode
        {
            get; set;
        }

        


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [DataType("AutoComplete/_Admin")]
        public virtual User LocationContact
        {
            get;
            set;
        }

        [DataType("AutoComplete/_UserId")]
        public int? LocationContactId { get; set; }


        [Column, Required]
        public string MarketableName { get; set; }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [Column]
        public Uri MarketableURL
        {
            get; set;
        }

        public string MarketableUrlString
        {
            get { return MarketableURL == null ? string.Empty : MarketableURL.ToString(); }
            set {
                MarketableURL = string.IsNullOrWhiteSpace(value) ? null : new Uri(value);
            }
        }

        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        public virtual User MarketingConsultant
        {
            get;
            set;
        }

        [DataType("AutoComplete/_UserId")]
        public int? MarketingConsultantId { get; set; }


        [Column]
        public string Message { get; set; }


        [Column]
        public string LocationDescription { get; set; }


        [Column, Required]
        public string Name { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private string _NameNoLocationCode = null;

        public string NameNoLocationCode
        {
            get { return this._NameNoLocationCode; }

        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private System.DateTime _OpenDate = DateTime.Now;
        [Column]
        [Required]
        public System.DateTime OpenDate
        {
            get { return this._OpenDate; }

            set { this._OpenDate = value; }
        }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "OperatorId", OtherKey = "UserId")]
        [DataType("AutoComplete/_Admin")]
        [UIHint("Badge/_User")]
        public virtual User Operator
        {
            get;
            set;
        }

        
        [DataType("AutoComplete/_UserId")]
        public int? OperatorId { get; set; }


        [Column]
        public string OperatorTeamName { get; set; }


        [Column]
        public string PriceGroupNumber { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>
        private System.DateTime _ProjectedOpenDate = DateTime.Now;
        [Column]
        [Required]
        public System.DateTime ProjectedOpenDate
        {
            get { return this._ProjectedOpenDate; }

            set { this._ProjectedOpenDate = value; }
        }


        [Column]
        public string RegionName { get; set; }


        [Column]
        public string ServiceTeamName { get; set; }


        /// <summary>
        /// Generated code from the Core Model Builder Add-on.
        /// </summary>
        /// <created>
        /// Generated Wednesday, March 14, 2012
        /// </created>

        [Column]
        [DataType("Enum/_StoreStatus")]
        public StoreStatus Status
        {
            get; set;
        }


        [Column]
        public string SiteStatus { get; set; }


        [System.Data.Linq.Mapping.Association(Name = "FK_User", ThisKey = "MarketingDirectorId", OtherKey = "UserId"), DataType("AutoComplete/_Admin")]
        public virtual User UnitMarketingDirector { get; set; }

        [DataType("AutoComplete/_UserId")]
        public int? UnitMarketingDirectorId { get; set; }


        public override string ToString()
        {
            var output = string.Format("{1} [{0},{2}]", this.LocationNumber, this.Name, this.MarketableName);
            if (this.StreetAddress == null)
                return output;
            return output+(string.IsNullOrEmpty(this.StreetAddress.Line1)?"":" "+this.StreetAddress);
        }

        public override string ToChecksum()
        {
            return ServiceTeamName + PriceGroupNumber; 
        }


        public string AnchorLabel()
        {
            string name = "View Store";
            if (IsBound())
                name = Name.ToString();
            return name;
        }
        public string AnchorHref()
        {
            return string.Format("/Admin/Store/Details/{0}", Id());
        } 
       

	    

        
	    


        

        public bool ChildcareAvailable { get; set; }
        public bool HasParking { get; set; }

        public string ParkingNotes { get; set; }
        public bool IsCorporateOffice { get; set; }
        public int MaximumCapacity { get; set; }

        public string Comments { get; set; }
    }
}
