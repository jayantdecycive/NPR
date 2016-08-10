namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hello_npr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketId = c.Int(nullable: false, identity: true),
                        OwnerId = c.Int(),
                        Note = c.String(),
                        SlotId = c.Int(nullable: false),
                        CreationSrc = c.String(),
                        CardNumber = c.Int(nullable: false),
                        GroupSize = c.Int(),
                        GroupType = c.String(),
                        GuestList = c.String(),
                        ContactNotes = c.String(),
                        Discriminator = c.String(maxLength: 128),
                        Item_MenuItemId = c.Int(),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.Slots", t => t.SlotId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.OwnerId)
                .ForeignKey("dbo.MenuItems", t => t.Item_MenuItemId)
                .Index(t => t.SlotId)
                .Index(t => t.OwnerId)
                .Index(t => t.Item_MenuItemId);
            
            CreateTable(
                "dbo.Slots",
                c => new
                    {
                        SlotId = c.Int(nullable: false, identity: true),
                        Grouping = c.Int(nullable: false),
                        TotalTickets = c.Int(nullable: false),
                        IsScheduled = c.Boolean(nullable: false),
                        StatusId = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Start = c.DateTimeOffset(nullable: false),
                        Cutoff = c.DateTimeOffset(nullable: false),
                        End = c.DateTimeOffset(nullable: false),
                        OccurrenceId = c.Int(nullable: false),
                        ScheduleId = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceId, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId)
                .Index(t => t.OccurrenceId)
                .Index(t => t.ScheduleId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Creation = c.DateTime(nullable: false),
                        LastActivity = c.DateTime(nullable: false),
                        AccountStatus = c.Int(nullable: false),
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
                        Discriminator = c.String(maxLength: 128),
                        Address_AddressId = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId)
                .Index(t => t.Address_AddressId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        LocationNumber = c.String(nullable: false, maxLength: 128),
                        BusinessConsultantId = c.Int(),
                        ConceptCodeId = c.Int(nullable: false),
                        CorporateOwned = c.Boolean(nullable: false),
                        BillingAddressId = c.Int(),
                        ShippingAddressId = c.Int(),
                        StreetAddressId = c.Int(),
                        PhoneString = c.String(),
                        FaxString = c.String(),
                        VoiceMailString = c.String(),
                        Email = c.String(nullable: false),
                        DistributorId = c.String(maxLength: 128),
                        Playground = c.String(nullable: false),
                        FinancialConsultantId = c.Int(),
                        GMTOffset = c.String(),
                        GMTOffsetApplied = c.Int(nullable: false),
                        GMTOffsetTimeZone = c.String(),
                        DayLightSavings = c.Boolean(nullable: false),
                        LocationCodeId = c.Int(nullable: false),
                        LocationContactId = c.Int(),
                        MarketableName = c.String(nullable: false),
                        MarketableUrlString = c.String(),
                        MarketingConsultantId = c.Int(),
                        Message = c.String(),
                        LocationDescription = c.String(),
                        Name = c.String(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        OperatorId = c.Int(),
                        OperatorTeamName = c.String(),
                        PriceGroupNumber = c.String(),
                        ProjectedOpenDate = c.DateTime(nullable: false),
                        RegionName = c.String(),
                        ServiceTeamName = c.String(),
                        StatusId = c.Int(nullable: false),
                        SiteStatus = c.String(),
                        UnitMarketingDirector_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.LocationNumber)
                .ForeignKey("dbo.Users", t => t.BusinessConsultantId)
                .ForeignKey("dbo.Addresses", t => t.BillingAddressId)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddressId)
                .ForeignKey("dbo.Addresses", t => t.StreetAddressId)
                .ForeignKey("dbo.Distributors", t => t.DistributorId)
                .ForeignKey("dbo.Users", t => t.FinancialConsultantId)
                .ForeignKey("dbo.Users", t => t.LocationContactId)
                .ForeignKey("dbo.Users", t => t.MarketingConsultantId)
                .ForeignKey("dbo.Users", t => t.OperatorId)
                .ForeignKey("dbo.Users", t => t.UnitMarketingDirector_UserId)
                .Index(t => t.BusinessConsultantId)
                .Index(t => t.BillingAddressId)
                .Index(t => t.ShippingAddressId)
                .Index(t => t.StreetAddressId)
                .Index(t => t.DistributorId)
                .Index(t => t.FinancialConsultantId)
                .Index(t => t.LocationContactId)
                .Index(t => t.MarketingConsultantId)
                .Index(t => t.OperatorId)
                .Index(t => t.UnitMarketingDirector_UserId);
            
            CreateTable(
                "dbo.Occurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false, identity: true),
                        ResEventId = c.Int(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        SlotRangeStart = c.DateTime(nullable: false),
                        SlotRangeEnd = c.DateTime(nullable: false),
                        BoundToPrototype = c.Boolean(nullable: false),
                        StoreId = c.String(nullable: false, maxLength: 128),
                        StatusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.ResEventId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.ResEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false, identity: true),
                        RegistrationStart = c.DateTime(nullable: false),
                        RegistrationEnd = c.DateTime(nullable: false),
                        SiteStart = c.DateTime(nullable: false),
                        SiteEnd = c.DateTime(nullable: false),
                        UrlName = c.String(nullable: false),
                        Url = c.String(),
                        Description = c.String(),
                        ReservationTypeId = c.String(nullable: false, maxLength: 128),
                        TemplateId = c.String(maxLength: 128),
                        Status = c.Int(nullable: false),
                        Name = c.String(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ReservationTypes", t => t.ReservationTypeId)
                .ForeignKey("dbo.ResTemplates", t => t.TemplateId)
                .Index(t => t.ReservationTypeId)
                .Index(t => t.TemplateId);
            
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
                "dbo.ResTemplates",
                c => new
                    {
                        ResTemplateId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        BrowserMedia = c.String(nullable: false),
                        DefaultReservationTypeId = c.String(),
                        Preview_MediaId = c.Int(),
                    })
                .PrimaryKey(t => t.ResTemplateId)
                .ForeignKey("dbo.Media", t => t.Preview_MediaId)
                .Index(t => t.Preview_MediaId);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        MediaId = c.Int(nullable: false, identity: true),
                        MediaUriStr = c.String(),
                        IsSystem = c.Boolean(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        FileSize = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        MediaType = c.Int(nullable: false),
                        ExternalUriStr = c.String(),
                        Owner_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.MediaId)
                .ForeignKey("dbo.Users", t => t.Owner_UserId)
                .Index(t => t.Owner_UserId);
            
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
                "dbo.MenuItems",
                c => new
                    {
                        MenuItemId = c.Int(nullable: false, identity: true),
                        DomId = c.String(),
                        Name = c.String(),
                        ImageUrl = c.String(),
                        ShortName = c.String(),
                        URLName = c.String(),
                    })
                .PrimaryKey(t => t.MenuItemId);
            
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
                "dbo.GiveawayEventMenuItems",
                c => new
                    {
                        GiveawayEvent_ResEventId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawayEvent_ResEventId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.GiveawayEvents", t => t.GiveawayEvent_ResEventId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawayEvent_ResEventId)
                .Index(t => t.MenuItem_MenuItemId);
            
            CreateTable(
                "dbo.GiveawayOccurrenceMenuItems",
                c => new
                    {
                        GiveawayOccurrence_OccurrenceId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawayOccurrence_OccurrenceId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.GiveawayOccurrences", t => t.GiveawayOccurrence_OccurrenceId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawayOccurrence_OccurrenceId)
                .Index(t => t.MenuItem_MenuItemId);
            
            CreateTable(
                "dbo.ResUserResStores",
                c => new
                    {
                        ResUser_UserId = c.Int(nullable: false),
                        ResStore_LocationNumber = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ResUser_UserId, t.ResStore_LocationNumber })
                .ForeignKey("dbo.ResUsers", t => t.ResUser_UserId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.ResStore_LocationNumber, cascadeDelete: true)
                .Index(t => t.ResUser_UserId)
                .Index(t => t.ResStore_LocationNumber);
            
            CreateTable(
                "dbo.GiveawaySlotMenuItems",
                c => new
                    {
                        GiveawaySlot_SlotId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawaySlot_SlotId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.Slots", t => t.GiveawaySlot_SlotId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawaySlot_SlotId)
                .Index(t => t.MenuItem_MenuItemId);
            
            CreateTable(
                "dbo.GiveawayEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .Index(t => t.ResEventId);
            
            CreateTable(
                "dbo.GiveawayOccurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false),
                        SampleProp = c.String(),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceId)
                .Index(t => t.OccurrenceId);
            
            CreateTable(
                "dbo.TourSlots",
                c => new
                    {
                        SlotId = c.Int(nullable: false),
                        Guide_UserId = c.Int(),
                        Cameos_CameoSetsId = c.String(maxLength: 128),
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
                        TourSlot_SlotId = c.Int(),
                        GuestCount = c.Int(nullable: false),
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
                .ForeignKey("dbo.TourSlots", t => t.TourSlot_SlotId)
                .Index(t => t.TicketId)
                .Index(t => t.TourSlot_SlotId);
            
            CreateTable(
                "dbo.ResUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        TourTicket_TicketId = c.Int(),
                        CameoSets_CameoSetsId = c.String(maxLength: 128),
                        CameoSets_CameoSetsId1 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId2 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId3 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId4 = c.String(maxLength: 128),
                        OperationRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.TourTickets", t => t.TourTicket_TicketId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId1)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId2)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId3)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId4)
                .Index(t => t.UserId)
                .Index(t => t.TourTicket_TicketId)
                .Index(t => t.CameoSets_CameoSetsId)
                .Index(t => t.CameoSets_CameoSetsId1)
                .Index(t => t.CameoSets_CameoSetsId2)
                .Index(t => t.CameoSets_CameoSetsId3)
                .Index(t => t.CameoSets_CameoSetsId4);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId4" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "TourTicket_TicketId" });
            DropIndex("dbo.ResUsers", new[] { "UserId" });
            DropIndex("dbo.TourTickets", new[] { "TourSlot_SlotId" });
            DropIndex("dbo.TourTickets", new[] { "TicketId" });
            DropIndex("dbo.TourSlots", new[] { "Cameos_CameoSetsId" });
            DropIndex("dbo.TourSlots", new[] { "Guide_UserId" });
            DropIndex("dbo.TourSlots", new[] { "SlotId" });
            DropIndex("dbo.GiveawayOccurrences", new[] { "OccurrenceId" });
            DropIndex("dbo.GiveawayEvents", new[] { "ResEventId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "GiveawaySlot_SlotId" });
            DropIndex("dbo.ResUserResStores", new[] { "ResStore_LocationNumber" });
            DropIndex("dbo.ResUserResStores", new[] { "ResUser_UserId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropIndex("dbo.Media", new[] { "Owner_UserId" });
            DropIndex("dbo.ResTemplates", new[] { "Preview_MediaId" });
            DropIndex("dbo.ResEvents", new[] { "TemplateId" });
            DropIndex("dbo.ResEvents", new[] { "ReservationTypeId" });
            DropIndex("dbo.Occurrences", new[] { "StoreId" });
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            DropIndex("dbo.Stores", new[] { "UnitMarketingDirector_UserId" });
            DropIndex("dbo.Stores", new[] { "OperatorId" });
            DropIndex("dbo.Stores", new[] { "MarketingConsultantId" });
            DropIndex("dbo.Stores", new[] { "LocationContactId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultantId" });
            DropIndex("dbo.Stores", new[] { "DistributorId" });
            DropIndex("dbo.Stores", new[] { "StreetAddressId" });
            DropIndex("dbo.Stores", new[] { "ShippingAddressId" });
            DropIndex("dbo.Stores", new[] { "BillingAddressId" });
            DropIndex("dbo.Stores", new[] { "BusinessConsultantId" });
            DropIndex("dbo.Users", new[] { "Address_AddressId" });
            DropIndex("dbo.Slots", new[] { "ScheduleId" });
            DropIndex("dbo.Slots", new[] { "OccurrenceId" });
            DropIndex("dbo.Tickets", new[] { "Item_MenuItemId" });
            DropIndex("dbo.Tickets", new[] { "OwnerId" });
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets");
            DropForeignKey("dbo.ResUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.TourTickets", "TourSlot_SlotId", "dbo.TourSlots");
            DropForeignKey("dbo.TourTickets", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users");
            DropForeignKey("dbo.TourSlots", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.Slots");
            DropForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences");
            DropForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropForeignKey("dbo.Media", "Owner_UserId", "dbo.Users");
            DropForeignKey("dbo.ResTemplates", "Preview_MediaId", "dbo.Media");
            DropForeignKey("dbo.ResEvents", "TemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "ReservationTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.Occurrences", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.Stores", "UnitMarketingDirector_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "OperatorId", "dbo.Users");
            DropForeignKey("dbo.Stores", "MarketingConsultantId", "dbo.Users");
            DropForeignKey("dbo.Stores", "LocationContactId", "dbo.Users");
            DropForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users");
            DropForeignKey("dbo.Stores", "DistributorId", "dbo.Distributors");
            DropForeignKey("dbo.Stores", "StreetAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "ShippingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BusinessConsultantId", "dbo.Users");
            DropForeignKey("dbo.Users", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Slots", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.Tickets", "OwnerId", "dbo.ResUsers");
            DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropTable("dbo.ResUsers");
            DropTable("dbo.TourTickets");
            DropTable("dbo.TourSlots");
            DropTable("dbo.GiveawayOccurrences");
            DropTable("dbo.GiveawayEvents");
            DropTable("dbo.GiveawaySlotMenuItems");
            DropTable("dbo.ResUserResStores");
            DropTable("dbo.GiveawayOccurrenceMenuItems");
            DropTable("dbo.GiveawayEventMenuItems");
            DropTable("dbo.Communications");
            DropTable("dbo.CameoSets");
            DropTable("dbo.Schedules");
            DropTable("dbo.Distributors");
            DropTable("dbo.MenuItems");
            DropTable("dbo.Addresses");
            DropTable("dbo.Media");
            DropTable("dbo.ResTemplates");
            DropTable("dbo.ReservationTypes");
            DropTable("dbo.ResEvents");
            DropTable("dbo.Occurrences");
            DropTable("dbo.Stores");
            DropTable("dbo.Users");
            DropTable("dbo.Slots");
            DropTable("dbo.Tickets");
        }
    }
}
