namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_bidirectional : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Occurrences", "Store_LocationNumber", "dbo.Stores");
            /*DropForeignKey("dbo.Stores", "BusinessConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "BillingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "ShippingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "StreetAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "Distributor_DistributorId", "dbo.Distributors");
            DropForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "LocationContact_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "MarketingConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "Operator_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "UnitMarketingDirector_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "ResEvent_ResEventId", "dbo.ResEvents");*/
            //DropIndex("dbo.Occurrences", new[] { "Store_LocationNumber" });
            /*DropIndex("dbo.Stores", new[] { "BusinessConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "BillingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "ShippingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "StreetAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "Distributor_DistributorId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "LocationContact_UserId" });
            DropIndex("dbo.Stores", new[] { "MarketingConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "Operator_UserId" });
            DropIndex("dbo.Stores", new[] { "UnitMarketingDirector_UserId" });
            DropIndex("dbo.Stores", new[] { "ResEvent_ResEventId" });*/
            
            /*CreateTable(
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
                        Playground = c.String(nullable: false),
                        GMTOffset = c.String(),
                        LocationCodeId = c.Int(nullable: false),
                        MarketableName = c.String(nullable: false),
                        MarketableUrlString = c.String(),
                        Message = c.String(),
                        LocationDescription = c.String(),
                        Name = c.String(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        OperatorTeamName = c.String(),
                        PriceGroupNumber = c.String(),
                        ProjectedOpenDate = c.DateTime(nullable: false),
                        RegionName = c.String(),
                        ServiceTeamName = c.String(),
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
                .Index(t => t.BusinessConsultant_UserId)
                .Index(t => t.BillingAddress_AddressId)
                .Index(t => t.ShippingAddress_AddressId)
                .Index(t => t.StreetAddress_AddressId)
                .Index(t => t.Distributor_DistributorId)
                .Index(t => t.FinancialConsultant_UserId)
                .Index(t => t.LocationContact_UserId)
                .Index(t => t.MarketingConsultant_UserId)
                .Index(t => t.Operator_UserId)
                .Index(t => t.UnitMarketingDirector_UserId);
            */
            CreateTable(
                "dbo.ResEventResStores",
                c => new
                    {
                        ResEvent_ResEventId = c.Int(nullable: false),
                        ResStore_LocationNumber = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ResEvent_ResEventId, t.ResStore_LocationNumber })
                .ForeignKey("dbo.ResEvents", t => t.ResEvent_ResEventId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.ResStore_LocationNumber, cascadeDelete: true)
                .Index(t => t.ResEvent_ResEventId)
                .Index(t => t.ResStore_LocationNumber);
            
            //AddForeignKey("dbo.Occurrences", "Store_LocationNumber", "dbo.Stores", "LocationNumber");
            //CreateIndex("dbo.Occurrences", "Store_LocationNumber");
            //DropTable("dbo.Stores");
        }
        
        public override void Down()
        {
            /*CreateTable(
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
                        Playground = c.String(nullable: false),
                        GMTOffset = c.String(),
                        LocationCodeId = c.Int(nullable: false),
                        MarketableName = c.String(nullable: false),
                        MarketableUrlString = c.String(),
                        Message = c.String(),
                        LocationDescription = c.String(),
                        Name = c.String(nullable: false),
                        OpenDate = c.DateTime(nullable: false),
                        OperatorTeamName = c.String(),
                        PriceGroupNumber = c.String(),
                        ProjectedOpenDate = c.DateTime(nullable: false),
                        RegionName = c.String(),
                        ServiceTeamName = c.String(),
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
                .PrimaryKey(t => t.LocationNumber);
            */
            DropIndex("dbo.ResEventResStores", new[] { "ResStore_LocationNumber" });
            DropIndex("dbo.ResEventResStores", new[] { "ResEvent_ResEventId" });
            /*DropIndex("dbo.Stores", new[] { "UnitMarketingDirector_UserId" });
            DropIndex("dbo.Stores", new[] { "Operator_UserId" });
            DropIndex("dbo.Stores", new[] { "MarketingConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "LocationContact_UserId" });
            DropIndex("dbo.Stores", new[] { "FinancialConsultant_UserId" });
            DropIndex("dbo.Stores", new[] { "Distributor_DistributorId" });
            DropIndex("dbo.Stores", new[] { "StreetAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "ShippingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "BillingAddress_AddressId" });
            DropIndex("dbo.Stores", new[] { "BusinessConsultant_UserId" });*/
            //DropIndex("dbo.Occurrences", new[] { "Store_LocationNumber" });
            DropForeignKey("dbo.ResEventResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.ResEventResStores", "ResEvent_ResEventId", "dbo.ResEvents");
            /*DropForeignKey("dbo.Stores", "UnitMarketingDirector_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "Operator_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "MarketingConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "LocationContact_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Stores", "Distributor_DistributorId", "dbo.Distributors");
            DropForeignKey("dbo.Stores", "StreetAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "ShippingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BillingAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Stores", "BusinessConsultant_UserId", "dbo.Users");
            DropForeignKey("dbo.Occurrences", "Store_LocationNumber", "dbo.Stores");*/
            DropTable("dbo.ResEventResStores");
            /*DropTable("dbo.Stores");
            CreateIndex("dbo.Stores", "ResEvent_ResEventId");
            CreateIndex("dbo.Stores", "UnitMarketingDirector_UserId");
            CreateIndex("dbo.Stores", "Operator_UserId");
            CreateIndex("dbo.Stores", "MarketingConsultant_UserId");
            CreateIndex("dbo.Stores", "LocationContact_UserId");
            CreateIndex("dbo.Stores", "FinancialConsultant_UserId");
            CreateIndex("dbo.Stores", "Distributor_DistributorId");
            CreateIndex("dbo.Stores", "StreetAddress_AddressId");
            CreateIndex("dbo.Stores", "ShippingAddress_AddressId");
            CreateIndex("dbo.Stores", "BillingAddress_AddressId");
            CreateIndex("dbo.Stores", "BusinessConsultant_UserId");
            CreateIndex("dbo.Occurrences", "Store_LocationNumber");
            AddForeignKey("dbo.Stores", "ResEvent_ResEventId", "dbo.ResEvents", "ResEventId");
            AddForeignKey("dbo.Stores", "UnitMarketingDirector_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Stores", "Operator_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Stores", "MarketingConsultant_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Stores", "LocationContact_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Stores", "FinancialConsultant_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Stores", "Distributor_DistributorId", "dbo.Distributors", "DistributorId");
            AddForeignKey("dbo.Stores", "StreetAddress_AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.Stores", "ShippingAddress_AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.Stores", "BillingAddress_AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.Stores", "BusinessConsultant_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.Occurrences", "Store_LocationNumber", "dbo.Stores", "LocationNumber");*/
        }
    }
}
