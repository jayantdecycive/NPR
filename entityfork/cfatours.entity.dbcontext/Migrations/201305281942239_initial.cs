namespace cfatours.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
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
                "dbo.ResEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false, identity: true),
                        IsFeatured = c.Boolean(nullable: false),
                        RegistrationStart = c.DateTime(nullable: false),
                        RegistrationEnd = c.DateTime(nullable: false),
                        SiteStart = c.DateTime(nullable: false),
                        SiteEnd = c.DateTime(nullable: false),
                        SubHeading = c.String(),
                        Description = c.String(),
                        MustBeOfAgeToAttend = c.Boolean(nullable: false),
                        ReservationTypeId = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.Int(),
                        TemplateId = c.String(maxLength: 128),
                        MediaId = c.Int(),
                        Status = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Name = c.String(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ReservationTypes", t => t.ReservationTypeId)
                .ForeignKey("dbo.ReservationCategories", t => t.CategoryId)
                .ForeignKey("dbo.ResTemplates", t => t.TemplateId)
                .ForeignKey("dbo.Media", t => t.MediaId)
                .Index(t => t.ReservationTypeId)
                .Index(t => t.CategoryId)
                .Index(t => t.TemplateId)
                .Index(t => t.MediaId);
            
            CreateTable(
                "dbo.Occurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        ResEventId = c.Int(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        SlotRangeStart = c.DateTime(nullable: false),
                        SlotRangeEnd = c.DateTime(nullable: false),
                        BoundToPrototype = c.Boolean(nullable: false),
                        StoreId = c.String(nullable: false, maxLength: 128),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.ResEventId)
                .Index(t => t.StoreId);
            
            CreateTable(
                "dbo.ResSiteUrls",
                c => new
                    {
                        SiteUrlId = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false, maxLength: 200),
                        ResEventId = c.Int(),
                        ResEventTypeId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SiteUrlId)
                .ForeignKey("dbo.ReservationTypes", t => t.ResEventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId, cascadeDelete: true)
                .Index(t => t.ResEventTypeId)
                .Index(t => t.ResEventId);
            
            CreateTable(
                "dbo.ReservationTypes",
                c => new
                    {
                        ReservationTypeId = c.String(nullable: false, maxLength: 128),
                        Description = c.String(nullable: false),
                        StrType = c.String(),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ReservationTypeId);
            
            CreateTable(
                "dbo.ReservationCategories",
                c => new
                    {
                        ReservationCategoryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ReservationCategoryId);
            
            CreateTable(
                "dbo.ResTemplates",
                c => new
                    {
                        ResTemplateId = c.String(nullable: false, maxLength: 128),
                        PreviewId = c.Int(),
                        Description = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        BrowserMedia = c.String(nullable: false),
                        DefaultReservationTypeId = c.String(),
                    })
                .PrimaryKey(t => t.ResTemplateId)
                .ForeignKey("dbo.Media", t => t.PreviewId)
                .Index(t => t.PreviewId);
            
            CreateTable(
                "dbo.Media",
                c => new
                    {
                        MediaId = c.Int(nullable: false, identity: true),
                        MediaUriStr = c.String(),
                        OwnerId = c.Int(),
                        IsSystem = c.Boolean(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Size = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false,defaultValueSql:"GETDATE()"),
                        FileSize = c.Long(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        MediaType = c.Int(nullable: false),
                        ExternalUriStr = c.String(),
                        CropX = c.Int(nullable: false),
                        CropY = c.Int(nullable: false),
                        CropWidth = c.Int(nullable: false),
                        CropHeight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MediaId)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false,defaultValueSql:"GETDATE()"),
                        LastActivity = c.DateTime(nullable: false),
                        AccountStatus = c.Int(nullable: false),
                        BirthDay = c.DateTime(nullable: false),
                        Email = c.String(nullable: false, maxLength: 250),
                        Username = c.String(maxLength: 250),
                        UID = c.String(),
                        DN = c.String(),
                        NameString = c.String(nullable: false),
                        HomePhoneString = c.String(),
                        MobilePhoneString = c.String(),
                        Authority = c.String(maxLength: 250),
                        AuthorityUID = c.String(maxLength: 250),
                        AddressId = c.Int(),
                        storeNumber = c.Int(),
                        employeeID = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        NameString = c.String(),
                        Line1 = c.String(maxLength: 100),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        ZipString = c.String(nullable: false, maxLength: 100),
                        County = c.String(),
                        State = c.String(),
                        City = c.String(),
                        Label = c.String(),
                    })
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        TicketId = c.Int(nullable: false, identity: true),
                        OwnerId = c.Int(),
                        Note = c.String(),
                        CreatedDate = c.DateTime(nullable: false,defaultValueSql:"GETDATE()"),
                        SlotId = c.Int(),
                        Status = c.Int(nullable: false),
                        CreationSrc = c.String(),
                        CardNumber = c.String(maxLength: 100,defaultValueSql:"NEWID()"),
                        TableRequest = c.String(),
                        NumberOfGuests = c.Int(),
                        MenuItemId = c.Int(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TicketId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .ForeignKey("dbo.ResUsers", t => t.OwnerId)
                .ForeignKey("dbo.MenuItems", t => t.MenuItemId, cascadeDelete: true)
                .Index(t => t.SlotId)
                .Index(t => t.OwnerId)
                .Index(t => t.MenuItemId);
            
            CreateTable(
                "dbo.Slots",
                c => new
                    {
                        SlotId = c.Int(nullable: false, identity: true),
                        IsScheduled = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Visibility = c.Int(nullable: false),
                        Start = c.DateTimeOffset(nullable: false),
                        Cutoff = c.DateTimeOffset(nullable: false),
                        End = c.DateTimeOffset(nullable: false),
                        OccurrenceId = c.Int(nullable: false),
                        ScheduleId = c.Int(),
                        Grouping = c.Int(nullable: false),
                        Notes = c.String(),
                        Discriminator = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceId, cascadeDelete: true)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId)
                .Index(t => t.OccurrenceId)
                .Index(t => t.ScheduleId);
            
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
                "dbo.Cameos",
                c => new
                    {
                        ResUserId = c.Int(nullable: false),
                        TourSlotId = c.Int(nullable: false),
                        CameoType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResUserId, t.TourSlotId })
                .ForeignKey("dbo.ResUsers", t => t.ResUserId, cascadeDelete: true)
                .ForeignKey("dbo.TourSlots", t => t.TourSlotId, cascadeDelete: true)
                .Index(t => t.ResUserId)
                .Index(t => t.TourSlotId);
            
            CreateTable(
                "dbo.TicketTransactions",
                c => new
                    {
                        TicketTransactionId = c.String(nullable: false, maxLength: 128,defaultValueSql:"NEWID()"),
                        CreateDate = c.DateTime(nullable: false,defaultValueSql:"GETDATE()"),
                        CustomerId = c.Int(),
                        Action = c.Int(nullable: false),
                        TicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TicketTransactionId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.CustomerId)
                .Index(t => t.TicketId)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.LocationSubscriptions",
                c => new
                    {
                        StoreId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        ReceiveEmails = c.Boolean(nullable: false),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreId, t.UserId })
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        LocationNumber = c.String(nullable: false, maxLength: 128),
                        CategoryId = c.String(maxLength: 128),
                        BusinessConsultantId = c.Int(),
                        ConceptCode = c.Int(nullable: false),
                        CorporateOwned = c.Boolean(nullable: false),
                        BillingAddressId = c.Int(),
                        ShippingAddressId = c.Int(),
                        StreetAddressId = c.Int(),
                        PhoneString = c.String(),
                        FaxString = c.String(),
                        VoiceMailString = c.String(),
                        Email = c.String(nullable: false),
                        DistributorId = c.String(maxLength: 128),
                        AcceptsCfaCard = c.Boolean(nullable: false),
                        HasDiningRoom = c.Boolean(nullable: false),
                        HasDriveThru = c.Boolean(nullable: false),
                        OffersOnlineOrdering = c.Boolean(nullable: false),
                        OffersWireless = c.Boolean(nullable: false),
                        Playground = c.String(nullable: false),
                        ServesBreakfast = c.Boolean(nullable: false),
                        FinancialConsultantId = c.Int(),
                        GMTOffset = c.String(),
                        GMTOffsetApplied = c.Int(nullable: false),
                        GMTOffsetTimeZone = c.String(),
                        DayLightSavings = c.Boolean(nullable: false),
                        Lat = c.Double(nullable: false),
                        Lon = c.Double(nullable: false),
                        LocationCode = c.Int(nullable: false),
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
                        Status = c.Int(nullable: false),
                        SiteStatus = c.String(),
                        UnitMarketingDirectorId = c.Int(),
                        ChildcareAvailable = c.Boolean(nullable: false),
                        HasParking = c.Boolean(nullable: false),
                        ParkingNotes = c.String(),
                        IsCorporateOffice = c.Boolean(nullable: false),
                        MaximumCapacity = c.Int(nullable: false),
                        Comments = c.String(),
                    })
                .PrimaryKey(t => t.LocationNumber)
                .ForeignKey("dbo.LocationCategories", t => t.CategoryId)
                .ForeignKey("dbo.Users", t => t.BusinessConsultantId)
                .ForeignKey("dbo.Addresses", t => t.BillingAddressId)
                .ForeignKey("dbo.Addresses", t => t.ShippingAddressId)
                .ForeignKey("dbo.Addresses", t => t.StreetAddressId)
                .ForeignKey("dbo.Distributors", t => t.DistributorId)
                .ForeignKey("dbo.Users", t => t.FinancialConsultantId)
                .ForeignKey("dbo.Users", t => t.LocationContactId)
                .ForeignKey("dbo.Users", t => t.MarketingConsultantId)
                .ForeignKey("dbo.Users", t => t.OperatorId)
                .ForeignKey("dbo.Users", t => t.UnitMarketingDirectorId)
                .Index(t => t.CategoryId)
                .Index(t => t.BusinessConsultantId)
                .Index(t => t.BillingAddressId)
                .Index(t => t.ShippingAddressId)
                .Index(t => t.StreetAddressId)
                .Index(t => t.DistributorId)
                .Index(t => t.FinancialConsultantId)
                .Index(t => t.LocationContactId)
                .Index(t => t.MarketingConsultantId)
                .Index(t => t.OperatorId)
                .Index(t => t.UnitMarketingDirectorId);
            
            CreateTable(
                "dbo.LocationCategories",
                c => new
                    {
                        LocationCategoryId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.LocationCategoryId);
            
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
                "dbo.ResTokens",
                c => new
                    {
                        TokenUID = c.Guid(nullable: false),
                        UserId = c.Int(nullable: false),
                        Action = c.String(),
                        Expiration = c.DateTime(nullable: false),
                        Data = c.String(),
                    })
                .PrimaryKey(t => t.TokenUID)
                .ForeignKey("dbo.ResUsers", t => t.UserId)
                .Index(t => t.UserId);
            
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
                "dbo.GiveawayEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                        MaxItems = c.Int(nullable: false),
                        MinItems = c.Int(nullable: false),
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
                        KidFriendly = c.Boolean(nullable: false),
                        SpecialNeeds = c.String(),
                        GuideId = c.Int(),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .ForeignKey("dbo.Users", t => t.GuideId)
                .Index(t => t.SlotId)
                .Index(t => t.GuideId);
            
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
                        EmailInsiders = c.Boolean(nullable: false),
                        OperationRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResUsers", new[] { "UserId" });
            DropIndex("dbo.TourTickets", new[] { "TourSlot_SlotId" });
            DropIndex("dbo.TourTickets", new[] { "TicketId" });
            DropIndex("dbo.TourSlots", new[] { "GuideId" });
            DropIndex("dbo.TourSlots", new[] { "SlotId" });
            DropIndex("dbo.GiveawayOccurrences", new[] { "OccurrenceId" });
            DropIndex("dbo.GiveawayEvents", new[] { "ResEventId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "GiveawaySlot_SlotId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropIndex("dbo.ResTokens", new[] { "UserId" });
            DropIndex("dbo.Stores", new[] { "UnitMarketingDirectorId" });
            DropIndex("dbo.Stores", new[] { "OperatorId" });
            DropIndex("dbo.Stores", new[] { "MarketingConsultantId" });
            DropIndex("dbo.Stores", new[] { "LocationContactId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultantId" });
            DropIndex("dbo.Stores", new[] { "DistributorId" });
            DropIndex("dbo.Stores", new[] { "StreetAddressId" });
            DropIndex("dbo.Stores", new[] { "ShippingAddressId" });
            DropIndex("dbo.Stores", new[] { "BillingAddressId" });
            DropIndex("dbo.Stores", new[] { "BusinessConsultantId" });
            DropIndex("dbo.Stores", new[] { "CategoryId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "UserId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "StoreId" });
            DropIndex("dbo.TicketTransactions", new[] { "CustomerId" });
            DropIndex("dbo.TicketTransactions", new[] { "TicketId" });
            DropIndex("dbo.Cameos", new[] { "TourSlotId" });
            DropIndex("dbo.Cameos", new[] { "ResUserId" });
            DropIndex("dbo.Slots", new[] { "ScheduleId" });
            DropIndex("dbo.Slots", new[] { "OccurrenceId" });
            DropIndex("dbo.Tickets", new[] { "MenuItemId" });
            DropIndex("dbo.Tickets", new[] { "OwnerId" });
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropIndex("dbo.Users", new[] { "AddressId" });
            DropIndex("dbo.Media", new[] { "OwnerId" });
            DropIndex("dbo.ResTemplates", new[] { "PreviewId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventTypeId" });
            DropIndex("dbo.Occurrences", new[] { "StoreId" });
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            DropIndex("dbo.ResEvents", new[] { "MediaId" });
            DropIndex("dbo.ResEvents", new[] { "TemplateId" });
            DropIndex("dbo.ResEvents", new[] { "CategoryId" });
            DropIndex("dbo.ResEvents", new[] { "ReservationTypeId" });
            DropForeignKey("dbo.ResUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.TourTickets", "TourSlot_SlotId", "dbo.TourSlots");
            DropForeignKey("dbo.TourTickets", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TourSlots", "GuideId", "dbo.Users");
            DropForeignKey("dbo.TourSlots", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.Slots");
            DropForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropForeignKey("dbo.ResTokens", "UserId", "dbo.ResUsers");
            DropForeignKey("dbo.Stores", "UnitMarketingDirectorId", "dbo.Users");
            DropForeignKey("dbo.Stores", "OperatorId", "dbo.Users");
            DropForeignKey("dbo.Stores", "MarketingConsultantId", "dbo.Users");
            DropForeignKey("dbo.Stores", "LocationContactId", "dbo.Users");
            DropForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users");
            DropForeignKey("dbo.Stores", "DistributorId", "dbo.Distributors");
            DropForeignKey("dbo.Stores", "StreetAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "ShippingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BillingAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BusinessConsultantId", "dbo.Users");
            DropForeignKey("dbo.Stores", "CategoryId", "dbo.LocationCategories");
            DropForeignKey("dbo.LocationSubscriptions", "UserId", "dbo.ResUsers");
            DropForeignKey("dbo.LocationSubscriptions", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.TicketTransactions", "CustomerId", "dbo.ResUsers");
            DropForeignKey("dbo.TicketTransactions", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.Cameos", "TourSlotId", "dbo.TourSlots");
            DropForeignKey("dbo.Cameos", "ResUserId", "dbo.ResUsers");
            DropForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Slots", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.Tickets", "OwnerId", "dbo.ResUsers");
            DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.Users", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Media", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.ResTemplates", "PreviewId", "dbo.Media");
            DropForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.Occurrences", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResEvents", "MediaId", "dbo.Media");
            DropForeignKey("dbo.ResEvents", "TemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "CategoryId", "dbo.ReservationCategories");
            DropForeignKey("dbo.ResEvents", "ReservationTypeId", "dbo.ReservationTypes");
            DropTable("dbo.ResUsers");
            DropTable("dbo.TourTickets");
            DropTable("dbo.TourSlots");
            DropTable("dbo.GiveawayOccurrences");
            DropTable("dbo.GiveawayEvents");
            DropTable("dbo.GiveawayOccurrenceMenuItems");
            DropTable("dbo.GiveawaySlotMenuItems");
            DropTable("dbo.GiveawayEventMenuItems");
            DropTable("dbo.ResTokens");
            DropTable("dbo.Communications");
            DropTable("dbo.Distributors");
            DropTable("dbo.LocationCategories");
            DropTable("dbo.Stores");
            DropTable("dbo.LocationSubscriptions");
            DropTable("dbo.TicketTransactions");
            DropTable("dbo.Cameos");
            DropTable("dbo.Schedules");
            DropTable("dbo.Slots");
            DropTable("dbo.Tickets");
            DropTable("dbo.Addresses");
            DropTable("dbo.Users");
            DropTable("dbo.Media");
            DropTable("dbo.ResTemplates");
            DropTable("dbo.ReservationCategories");
            DropTable("dbo.ReservationTypes");
            DropTable("dbo.ResSiteUrls");
            DropTable("dbo.Occurrences");
            DropTable("dbo.ResEvents");
            DropTable("dbo.MenuItems");
        }
    }
}
