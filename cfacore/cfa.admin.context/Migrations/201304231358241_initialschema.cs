namespace cfa.admin.context.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialschema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stories",
                c => new
                    {
                        StoryId = c.Int(nullable: false, identity: true),
                        Slug = c.String(maxLength: 100),
                        TagLine = c.String(maxLength: 200),
                        Title = c.String(nullable: false, maxLength: 200),
                        CreatedOn = c.DateTime(nullable: false),
                        CampaignId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StoryId)
                .ForeignKey("dbo.Campaigns", t => t.CampaignId, cascadeDelete: true)
                .Index(t => t.CampaignId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Slug = c.String(maxLength: 100),
                        KeyWords = c.String(maxLength: 500),
                        CreatedOn = c.DateTime(nullable: false),
                        ComMedia_MediaId = c.Int(),
                        Campaign_CampaignId = c.Int(),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.ComMedias", t => t.ComMedia_MediaId)
                .ForeignKey("dbo.Campaigns", t => t.Campaign_CampaignId)
                .Index(t => t.ComMedia_MediaId)
                .Index(t => t.Campaign_CampaignId);
            
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        CampaignId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Slug = c.String(maxLength: 200),
                        CreatedOn = c.DateTime(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(),
                    })
                .PrimaryKey(t => t.CampaignId);
            
            CreateTable(
                "dbo.ComMedias",
                c => new
                    {
                        MediaId = c.Int(nullable: false, identity: true),
                        CropX = c.Int(nullable: false),
                        CropY = c.Int(nullable: false),
                        CropWidth = c.Int(nullable: false),
                        CropHeight = c.Int(nullable: false),
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
                        Campaign_CampaignId = c.Int(),
                    })
                .PrimaryKey(t => t.MediaId)
                .ForeignKey("dbo.Users", t => t.Owner_UserId)
                .ForeignKey("dbo.Campaigns", t => t.Campaign_CampaignId)
                .Index(t => t.Owner_UserId)
                .Index(t => t.Campaign_CampaignId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false),
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
                        ImageId = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.ComMedias", t => t.ImageId)
                .Index(t => t.AddressId)
                .Index(t => t.ImageId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        Line2 = c.String(),
                        Line3 = c.String(),
                        ZipString = c.String(nullable: false, maxLength: 100),
                        County = c.String(),
                        State = c.String(),
                        Line1 = c.String(maxLength: 100),
                        City = c.String(),
                        Label = c.String(),
                        NameString = c.String(),
                    })
                .PrimaryKey(t => t.AddressId);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        LocationNumber = c.String(nullable: false, maxLength: 128),
                        OpeningReleaseId = c.Int(),
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
                        MarketableName = c.String(nullable: false, maxLength: 200),
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
                        OpeningRelease_StoryId = c.Int(),
                    })
                .PrimaryKey(t => t.LocationNumber)
                .ForeignKey("dbo.Stories", t => t.OpeningRelease_StoryId)
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
                .Index(t => t.OpeningRelease_StoryId)
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
                "dbo.StoryTags",
                c => new
                    {
                        Story_StoryId = c.Int(nullable: false),
                        Tag_TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Story_StoryId, t.Tag_TagId })
                .ForeignKey("dbo.Stories", t => t.Story_StoryId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.Tag_TagId, cascadeDelete: true)
                .Index(t => t.Story_StoryId)
                .Index(t => t.Tag_TagId);
            
            CreateTable(
                "dbo.StoryComMedias",
                c => new
                    {
                        Story_StoryId = c.Int(nullable: false),
                        ComMedia_MediaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Story_StoryId, t.ComMedia_MediaId })
                .ForeignKey("dbo.Stories", t => t.Story_StoryId, cascadeDelete: true)
                .ForeignKey("dbo.ComMedias", t => t.ComMedia_MediaId, cascadeDelete: true)
                .Index(t => t.Story_StoryId)
                .Index(t => t.ComMedia_MediaId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.StoryComMedias", new[] { "ComMedia_MediaId" });
            DropIndex("dbo.StoryComMedias", new[] { "Story_StoryId" });
            DropIndex("dbo.StoryTags", new[] { "Tag_TagId" });
            DropIndex("dbo.StoryTags", new[] { "Story_StoryId" });
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
            DropIndex("dbo.Stores", new[] { "OpeningRelease_StoryId" });
            DropIndex("dbo.Users", new[] { "ImageId" });
            DropIndex("dbo.Users", new[] { "AddressId" });
            DropIndex("dbo.ComMedias", new[] { "Campaign_CampaignId" });
            DropIndex("dbo.ComMedias", new[] { "Owner_UserId" });
            DropIndex("dbo.Tags", new[] { "Campaign_CampaignId" });
            DropIndex("dbo.Tags", new[] { "ComMedia_MediaId" });
            DropIndex("dbo.Stories", new[] { "CampaignId" });
            DropForeignKey("dbo.StoryComMedias", "ComMedia_MediaId", "dbo.ComMedias");
            DropForeignKey("dbo.StoryComMedias", "Story_StoryId", "dbo.Stories");
            DropForeignKey("dbo.StoryTags", "Tag_TagId", "dbo.Tags");
            DropForeignKey("dbo.StoryTags", "Story_StoryId", "dbo.Stories");
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
            DropForeignKey("dbo.Stores", "OpeningRelease_StoryId", "dbo.Stories");
            DropForeignKey("dbo.Users", "ImageId", "dbo.ComMedias");
            DropForeignKey("dbo.Users", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.ComMedias", "Campaign_CampaignId", "dbo.Campaigns");
            DropForeignKey("dbo.ComMedias", "Owner_UserId", "dbo.Users");
            DropForeignKey("dbo.Tags", "Campaign_CampaignId", "dbo.Campaigns");
            DropForeignKey("dbo.Tags", "ComMedia_MediaId", "dbo.ComMedias");
            DropForeignKey("dbo.Stories", "CampaignId", "dbo.Campaigns");
            DropTable("dbo.StoryComMedias");
            DropTable("dbo.StoryTags");
            DropTable("dbo.Distributors");
            DropTable("dbo.Stores");
            DropTable("dbo.Addresses");
            DropTable("dbo.Users");
            DropTable("dbo.ComMedias");
            DropTable("dbo.Campaigns");
            DropTable("dbo.Tags");
            DropTable("dbo.Stories");
        }
    }
}
