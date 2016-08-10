using System;
using cfacore.domain.user;
using cfacore.shared.domain.store;
using cfacore.domain._base;
using cfacore.shared.domain.user;
using System.ComponentModel;

namespace cfacore.domain.store
{
    public interface IStore : IDomainObject
    {
        string LocationNumber { get; set; }

        User BusinessConsultant { get; set; }

        ConceptCode ConceptCode { get; set; }

        bool CorporateOwned { get; set; }
        
        Address BillingAddress { get; set; }
        
        Address ShippingAddress { get; set; }
        
        Address StreetAddress { get; set; }
       
        Phone Phone { get; set; }

        Phone Fax { get; set; }

        Phone VoiceMail { get; set; }

        string Email { get; set; }

        Distributor Distributor { get; set; }
        
        StoreFeatures Features { get; }

        bool AcceptsCfaCard { get; }
 
        bool HasDiningRoom { get; }
        
        bool HasDriveThru { get; }

        bool OffersOnlineOrdering { get; }

        bool OffersWireless { get;  }
    
        string Playground { get; }
     
        bool ServesBreakfast { get; }
        
        User FinancialConsultant { get; set; }
        
        string GMTOffset { get; set; }
        
        GeographicCoordinate Coordinates { get; set; }
      
        double Lat { get; }
      
        double Lon { get; }

        User LocationContact { get; set; }
        
        string MarketableName { get; set; }

        Uri MarketableURL { get; set; }

        User MarketingConsultant { get; set; }

        string Message { get; set; }

        string LocationDescription { get; set; }

        string Name { get; set; }        
        string NameNoLocationCode { get; }
        
        DateTime OpenDate { get; set; }

        User Operator { get; set; }

        string OperatorTeamName { get; set; }
        
        string PriceGroupNumber { get; set; }
        
        DateTime ProjectedOpenDate { get; set; }
        
        string RegionName { get; set; }
        
        string ServiceTeamName { get; set; }
        
        StoreStatus Status { get; set; }

        string SiteStatus { get; set; }

        LocationCode LocationCode { get; set; }
        
        User UnitMarketingDirector { get; set; }

	    
        bool ChildcareAvailable { get; set; }

	    bool HasParking { get; set; }
	    string ParkingNotes { get; set; }

	    bool IsCorporateOffice { get; set; }

	    int MaximumCapacity { get; set; }

	    

        string Comments { get; set; }
    }

    public enum LocationCode
    {     
        [Description("Airport")]
        AIR,
        [Description("College")]
        COLL,
        [Description("DINC")]
        DINC,
        [Description("Drive Thru Only")]
        DTO,
        [Description("Free Standing Restaurant")]
        FSU,        
        HJG,
        [Description("Hospital")]
        HOSP,
        KRG,
        [Description("Mall")]
        MALL,
        OFFC,
        TDNC
    }

    public enum ConceptCode
    {
        [Description("Chick-fil-A")]
        CFA,
        [Description("Dwarf House")]
        DWH,
        [Description("License")]
        LIC
    }

    public enum StoreStatus
    {
        [Description("Open")]
        open,
        [Description("Future")]
        future,
        [Description("Remodel")]
        remodel
    }

    
}
