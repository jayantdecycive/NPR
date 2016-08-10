namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false, identity: true),
                        RegistrationAvailability_Start = c.DateTime(nullable: false),
                        RegistrationAvailability_End = c.DateTime(nullable: false),
                        RegistrationAvailability_Span = c.Time(nullable: false),
                        SiteAvailability_Start = c.DateTime(nullable: false),
                        SiteAvailability_End = c.DateTime(nullable: false),
                        SiteAvailability_Span = c.Time(nullable: false),
                        UrlName = c.String(nullable: false),
                        Description = c.String(),
                        TemplateName = c.String(),
                        MobileTemplateName = c.String(),
                        TabletTemplateName = c.String(),
                        StatusId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        ReservationType_ReservationTypeId = c.String(nullable: false, maxLength: 128),
                        Template_ResTemplateId = c.String(maxLength: 128),
                        MobileTemplate_ResTemplateId = c.String(maxLength: 128),
                        TabletTemplate_ResTemplateId = c.String(maxLength: 128),
                        ProtoOccurrence_OccurrenceId = c.Int(),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ReservationTypes", t => t.ReservationType_ReservationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ResTemplates", t => t.Template_ResTemplateId)
                .ForeignKey("dbo.ResTemplates", t => t.MobileTemplate_ResTemplateId)
                .ForeignKey("dbo.ResTemplates", t => t.TabletTemplate_ResTemplateId)
                .ForeignKey("dbo.Occurrences", t => t.ProtoOccurrence_OccurrenceId)
                .Index(t => t.ReservationType_ReservationTypeId)
                .Index(t => t.Template_ResTemplateId)
                .Index(t => t.MobileTemplate_ResTemplateId)
                .Index(t => t.TabletTemplate_ResTemplateId)
                .Index(t => t.ProtoOccurrence_OccurrenceId);
            
            CreateTable(
                "dbo.Slots",
                c => new
                    {
                        SlotId = c.Int(nullable: false, identity: true),
                        Availability_Start = c.DateTime(nullable: false),
                        Availability_End = c.DateTime(nullable: false),
                        Availability_Span = c.Time(nullable: false),
                        TotalTickets = c.Int(nullable: false),
                        IsScheduled = c.Boolean(nullable: false),
                        StatusId = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Start = c.DateTime(nullable: false),
                        Cutoff = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        OccurrenceId = c.String(),
                        ScheduleId = c.Int(nullable: false),
                        Discriminator = c.String(maxLength: 128),
                        ResEvent_ResEventId = c.Int(),
                        Occurrence_OccurrenceId = c.Int(),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .ForeignKey("dbo.ResEvents", t => t.ResEvent_ResEventId)
                .ForeignKey("dbo.Occurrences", t => t.Occurrence_OccurrenceId)
                .Index(t => t.ScheduleId)
                .Index(t => t.ResEvent_ResEventId)
                .Index(t => t.Occurrence_OccurrenceId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketId = c.Int(nullable: false, identity: true),
                        Note = c.String(),
                        SlotId = c.Int(nullable: false),
                        CreationSrc = c.String(),
                        CardNumber = c.Int(nullable: false),
                        Discriminator = c.String(maxLength: 128),
                        Owner_UserId = c.Int(),
                        ReservationType_ReservationTypeId = c.String(maxLength: 128),
                        Occurrence_OccurrenceId = c.Int(),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.Users", t => t.Owner_UserId)
                .ForeignKey("dbo.Slots", t => t.SlotId, cascadeDelete: true)
                .ForeignKey("dbo.ReservationTypes", t => t.ReservationType_ReservationTypeId)
                .ForeignKey("dbo.Occurrences", t => t.Occurrence_OccurrenceId)
                .Index(t => t.Owner_UserId)
                .Index(t => t.SlotId)
                .Index(t => t.ReservationType_ReservationTypeId)
                .Index(t => t.Occurrence_OccurrenceId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Creation = c.DateTime(nullable: false),
                        LastActivity = c.DateTime(nullable: false),
                        Email = c.String(nullable: false),
                        Username = c.String(),
                        UID = c.String(),
                        DN = c.String(),
                        NameString = c.String(nullable: false),
                        HomePhoneString = c.String(),
                        MobilePhoneString = c.String(),
                        Authority = c.String(),
                        AuthorityUID = c.String(),
                        storeNumber = c.Int(),
                        employeeID = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        TourTicket_TicketId = c.Int(),
                        CameoSets_CameoSetsId = c.String(maxLength: 128),
                        CameoSets_CameoSetsId1 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId2 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId3 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId4 = c.String(maxLength: 128),
                        Address_AddressId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.TourTickets", t => t.TourTicket_TicketId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId1)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId2)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId3)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId4)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId)
                .Index(t => t.TourTicket_TicketId)
                .Index(t => t.CameoSets_CameoSetsId)
                .Index(t => t.CameoSets_CameoSetsId1)
                .Index(t => t.CameoSets_CameoSetsId2)
                .Index(t => t.CameoSets_CameoSetsId3)
                .Index(t => t.CameoSets_CameoSetsId4)
                .Index(t => t.Address_AddressId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        ZipString = c.String(),
                        County = c.String(),
                        State = c.String(),
                        Line1 = c.String(maxLength: 100),
                        City = c.String(),
                        Label = c.String(),
                        NameString = c.String(),
                    })
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        ScheduleId = c.Int(nullable: false, identity: true),
                        TimeRange_Start = c.DateTime(nullable: false),
                        TimeRange_End = c.DateTime(nullable: false),
                        TimeRange_Span = c.Time(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Name = c.String(),
                        UrlName = c.String(),
                        Region = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScheduleId);
            
            CreateTable(
                "dbo.CameoSets",
                c => new
                    {
                        CameoSetsId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CameoSetsId);
            
            CreateTable(
                "dbo.ReservationTypes",
                c => new
                    {
                        ReservationTypeId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        StrType = c.String(),
                        Name = c.String(nullable: false),
                        UrlName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ReservationTypeId);
            
            CreateTable(
                "dbo.Occurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        SlotRangeStart = c.DateTime(nullable: false),
                        SlotRangeEnd = c.DateTime(nullable: false),
                        BoundToPrototype = c.Boolean(nullable: false),
                        StatusId = c.Int(nullable: false),
                        StoreId = c.String(),
                        RegistrationAvailability_Start = c.DateTime(nullable: false),
                        RegistrationAvailability_End = c.DateTime(nullable: false),
                        RegistrationAvailability_Span = c.Time(nullable: false),
                        SlotRange_Start = c.DateTime(nullable: false),
                        SlotRange_End = c.DateTime(nullable: false),
                        SlotRange_Span = c.Time(nullable: false),
                        Store_LocationNumber = c.String(maxLength: 128),
                        ReservationType_ReservationTypeId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.Stores", t => t.Store_LocationNumber)
                .ForeignKey("dbo.ReservationTypes", t => t.ReservationType_ReservationTypeId)
                .Index(t => t.Store_LocationNumber)
                .Index(t => t.ReservationType_ReservationTypeId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        LocationNumber = c.String(nullable: false, maxLength: 128),
                        ConceptCodeId = c.Int(nullable: false),
                        CorporateOwned = c.Boolean(nullable: false),
                        PhoneString = c.String(),
                        FaxString = c.String(),
                        VoiceMailString = c.String(),
                        Email = c.String(nullable: false),
                        GMTOffset = c.String(),
                        LocationCodeId = c.Int(nullable: false),
                        MarketableName = c.String(nullable: false),
                        MarketableUrlString = c.String(),
                        Message = c.String(nullable: false),
                        LocationDescription = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        OperatorTeamName = c.String(nullable: false),
                        PriceGroupNumber = c.String(nullable: false),
                        ProjectedOpenDate = c.DateTime(nullable: false),
                        RegionName = c.String(nullable: false),
                        ServiceTeamName = c.String(nullable: false),
                        StatusId = c.Int(nullable: false),
                        SiteStatus = c.String(),
                        BusinessConsultant_UserId = c.Int(),
                        BillingAddress_AddressId = c.Int(),
                        ShippingAddress_AddressId = c.Int(),
                        StreetAddress_AddressId = c.Int(),
                        Distributor_DistributorId = c.String(maxLength: 128),
                        FinancialConsultant_UserId = c.Int(),
                        LocationContact_UserId = c.Int(),
                        MarketingConsultant_UserId = c.Int(),
                        Operator_UserId = c.Int(),
                        UnitMarketingDirector_UserId = c.Int(),
                        ResEvent_ResEventId = c.Int(),
                    })
                .PrimaryKey(t => t.LocationNumber)
                .ForeignKey("dbo.Users", t => t.BusinessConsultant_UserId)
                .ForeignKey("dbo.Addresses", t => t.BillingAddress_AddressId)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddress_AddressId)
                .ForeignKey("dbo.Addresses", t => t.StreetAddress_AddressId)
                .ForeignKey("dbo.Distributors", t => t.Distributor_DistributorId)
                .ForeignKey("dbo.Users", t => t.FinancialConsultant_UserId)
                .ForeignKey("dbo.Users", t => t.LocationContact_UserId)
                .ForeignKey("dbo.Users", t => t.MarketingConsultant_UserId)
                .ForeignKey("dbo.Users", t => t.Operator_UserId)
                .ForeignKey("dbo.Users", t => t.UnitMarketingDirector_UserId)
                .ForeignKey("dbo.ResEvents", t => t.ResEvent_ResEventId)
                .Index(t => t.BusinessConsultant_UserId)
                .Index(t => t.BillingAddress_AddressId)
                .Index(t => t.ShippingAddress_AddressId)
                .Index(t => t.StreetAddress_AddressId)
                .Index(t => t.Distributor_DistributorId)
                .Index(t => t.FinancialConsultant_UserId)
                .Index(t => t.LocationContact_UserId)
                .Index(t => t.MarketingConsultant_UserId)
                .Index(t => t.Operator_UserId)
                .Index(t => t.UnitMarketingDirector_UserId)
                .Index(t => t.ResEvent_ResEventId);
            
            CreateTable(
                "dbo.Distributors",
                c => new
                    {
                        DistributorId = c.String(nullable: false, maxLength: 128),
                        DistributionCenter = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        ShortName = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.DistributorId);
            
            CreateTable(
                "dbo.ResTemplates",
                c => new
                    {
                        ResTemplateId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        BrowserMedia = c.String(nullable: false),
                        DefaultReservationTypeId = c.String(),
                        Icon_MediaId = c.String(maxLength: 128),
                        Preview_MediaId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ResTemplateId)
                .ForeignKey("dbo.Media", t => t.Icon_MediaId)
                .ForeignKey("dbo.Media", t => t.Preview_MediaId)
                .Index(t => t.Icon_MediaId)
                .Index(t => t.Preview_MediaId);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        MediaId = c.String(nullable: false, maxLength: 128),
                        MediaUriStr = c.String(),
                        OwnerId = c.String(),
                        IsSystem = c.Boolean(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        FileSize = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.MediaId);
            
            CreateTable(
                "dbo.Communications",
                c => new
                    {
                        CommunicationId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        EmailUriString = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CommunicationId);
            
            CreateTable(
                "dbo.TourSlots",
                c => new
                    {
                        SlotId = c.Int(nullable: false),
                        Guide_UserId = c.Int(),
                        Cameos_CameoSetsId = c.String(maxLength: 128),
                        TourSlotId = c.Int(nullable: false),
                        KidFriendly = c.Boolean(nullable: false),
                        SpecialNeeds = c.String(),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .ForeignKey("dbo.Users", t => t.Guide_UserId)
                .ForeignKey("dbo.CameoSets", t => t.Cameos_CameoSetsId)
                .Index(t => t.SlotId)
                .Index(t => t.Guide_UserId)
                .Index(t => t.Cameos_CameoSetsId);
            
            CreateTable(
                "dbo.TourTickets",
                c => new
                    {
                        TicketId = c.Int(nullable: false),
                        GuestCount = c.Int(nullable: false),
                        TourTicketId = c.String(),
                        TotalCostOfLunches = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OptInForLunch = c.Boolean(nullable: false),
                        NumberOfAdultLunches = c.Int(nullable: false),
                        NumberOfKidLunches = c.Int(nullable: false),
                        NumberOfSpecialNeedLunches = c.Int(nullable: false),
                        SpecialDietNeedsDescription = c.String(maxLength: 250),
                        VisitMarketing = c.Boolean(nullable: false),
                        VisitTech = c.Boolean(nullable: false),
                        VisitInnovation = c.Boolean(nullable: false),
                        VisitTraining = c.Boolean(nullable: false),
                        VisitWellness = c.Boolean(nullable: false),
                        VisitWareHouse = c.Boolean(nullable: false),
                        VisitIT = c.Boolean(nullable: false),
                        VisitOther = c.Boolean(nullable: false),
                        VisitOtherDescription = c.String(),
                        HasSpecialNeeds = c.Boolean(nullable: false),
                        IsVisuallyImpaired = c.Boolean(nullable: false),
                        OtherNeeds = c.Boolean(nullable: false),
                        OtherNeedsDescription = c.String(maxLength: 250),
                        IsHearingImpaired = c.Boolean(nullable: false),
                        NeedsWheelChair = c.Boolean(nullable: false),
                        IsFamilyWithKids = c.Boolean(nullable: false),
                        IsSchoolGroup = c.Boolean(nullable: false),
                        IsFamilyWithoutKids = c.Boolean(nullable: false),
                        IsAdultGroup = c.Boolean(nullable: false),
                        IsReligiousGroup = c.Boolean(nullable: false),
                        IsSeniorGroup = c.Boolean(nullable: false),
                        IsBusinessGroup = c.Boolean(nullable: false),
                        IsRavingFans = c.Boolean(nullable: false),
                        IsTeamMemberGroup = c.Boolean(nullable: false),
                        IsOtherTypeOfGroup = c.Boolean(nullable: false),
                        OtherTypeDescription = c.String(maxLength: 250),
                        GroupName = c.String(nullable: false),
                        GuestList = c.String(),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.Tickets", t => t.TicketId)
                .Index(t => t.TicketId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TourTickets", new[] { "TicketId" });
            DropIndex("dbo.TourSlots", new[] { "Cameos_CameoSetsId" });
            DropIndex("dbo.TourSlots", new[] { "Guide_UserId" });
            DropIndex("dbo.TourSlots", new[] { "SlotId" });
            DropIndex("dbo.ResTemplates", new[] { "Preview_MediaId" });
            DropIndex("dbo.ResTemplates", new[] { "Icon_MediaId" });
            DropIndex("dbo.Stores", new[] { "ResEvent_ResEventId" });
            DropIndex("dbo.Stores", new[] { "UnitMarketingDirector_UserId" });
            DropIndex("dbo.Stores", new[] { "Operator_UserId" });
            DropIndex("dbo.Stores", new[] { "MarketingConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "LocationContact_UserId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "Distributor_DistributorId" });
            DropIndex("dbo.Stores", new[] { "StreetAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "ShippingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "BillingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "BusinessConsultant_UserId" });
            DropIndex("dbo.Occurrences", new[] { "ReservationType_ReservationTypeId" });
            DropIndex("dbo.Occurrences", new[] { "Store_LocationNumber" });
            DropIndex("dbo.Users", new[] { "Address_AddressId" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId4" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.Users", new[] { "TourTicket_TicketId" });
            DropIndex("dbo.Tickets", new[] { "Occurrence_OccurrenceId" });
            DropIndex("dbo.Tickets", new[] { "ReservationType_ReservationTypeId" });
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropIndex("dbo.Tickets", new[] { "Owner_UserId" });
            DropIndex("dbo.Slots", new[] { "Occurrence_OccurrenceId" });
            DropIndex("dbo.Slots", new[] { "ResEvent_ResEventId" });
            DropIndex("dbo.Slots", new[] { "ScheduleId" });
            DropIndex("dbo.ResEvents", new[] { "ProtoOccurrence_OccurrenceId" });
            DropIndex("dbo.ResEvents", new[] { "TabletTemplate_ResTemplateId" });
            DropIndex("dbo.ResEvents", new[] { "MobileTemplate_ResTemplateId" });
            DropIndex("dbo.ResEvents", new[] { "Template_ResTemplateId" });
            DropIndex("dbo.ResEvents", new[] { "ReservationType_ReservationTypeId" });
            DropForeignKey("dbo.TourTickets", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users");
            DropForeignKey("dbo.TourSlots", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.ResTemplates", "Preview_MediaId", "dbo.Media");
            DropForeignKey("dbo.ResTemplates", "Icon_MediaId", "dbo.Media");
            DropForeignKey("dbo.Stores", "ResEvent_ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.Stores", "UnitMarketingDirector_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "Operator_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "MarketingConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "LocationContact_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "Distributor_DistributorId", "dbo.Distributors");
            DropForeignKey("dbo.Stores", "StreetAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "ShippingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BillingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BusinessConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Occurrences", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.Occurrences", "Store_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.Users", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "TourTicket_TicketId", "dbo.TourTickets");
            DropForeignKey("dbo.Tickets", "Occurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Tickets", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.Tickets", "Owner_UserId", "dbo.Users");
            DropForeignKey("dbo.Slots", "Occurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Slots", "ResEvent_ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.ResEvents", "ProtoOccurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.ResEvents", "TabletTemplate_ResTemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "MobileTemplate_ResTemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "Template_ResTemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "ReservationType_ReservationTypeId", "dbo.ReservationTypes");
            DropTable("dbo.TourTickets");
            DropTable("dbo.TourSlots");
            DropTable("dbo.Communications");
            DropTable("dbo.Media");
            DropTable("dbo.ResTemplates");
            DropTable("dbo.Distributors");
            DropTable("dbo.Stores");
            DropTable("dbo.Occurrences");
            DropTable("dbo.ReservationTypes");
            DropTable("dbo.CameoSets");
            DropTable("dbo.Schedules");
            DropTable("dbo.Addresses");
            DropTable("dbo.Users");
            DropTable("dbo.Tickets");
            DropTable("dbo.Slots");
            DropTable("dbo.ResEvents");
        }
    }
}
