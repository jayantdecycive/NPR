namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relation_update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers");
            DropForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users");
            DropForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropIndex("dbo.ResUserResStores", new[] { "ResUser_UserId" });
            DropIndex("dbo.ResUserResStores", new[] { "ResStore_LocationNumber" });
            DropIndex("dbo.TourSlots", new[] { "Guide_UserId" });
            DropIndex("dbo.TourSlots", new[] { "Cameos_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "TourTicket_TicketId" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId4" });
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirector_UserId", newName: "UnitMarketingDirectorId");
            //RenameColumn(table: "dbo.Cameos", name: "Cameos_CameoSetsId", newName: "TourSlotId");
            CreateTable(
                "dbo.LocationSubscriptions",
                c => new
                    {
                        StoreId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                        ReceiveEmails = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.StoreId, t.UserId })
                .ForeignKey("dbo.Stores", t => t.StoreId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.StoreId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.NPRLocations",
                c => new
                    {
                        NPRLocationId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SiteName = c.String(),
                        EventSpaceType = c.Int(nullable: false),
                        Parking = c.Boolean(nullable: false),
                        AtNPR = c.Boolean(nullable: false),
                        Capacity = c.Int(nullable: false),
                        ContactName = c.String(),
                        Phone_Number = c.Int(nullable: false),
                        Phone_Full = c.Long(nullable: false),
                        Phone_AreaCode = c.Int(nullable: false),
                        Phone_Extension = c.String(),
                        Phone_Type = c.String(),
                        Phone_Carrier = c.String(),
                        Email = c.String(),
                        Comments = c.String(),
                        AddressId = c.Int(),
                    })
                .PrimaryKey(t => t.NPRLocationId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .Index(t => t.AddressId);
            
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
            
            AddColumn("dbo.Tickets", "FirstName", c => c.String());
            AddColumn("dbo.Tickets", "LastName", c => c.String());
            AddColumn("dbo.Tickets", "Email", c => c.String());
            AddColumn("dbo.Tickets", "EmailConfirm", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Number", c => c.Int());
            AddColumn("dbo.Tickets", "Phone_Full", c => c.Long());
            AddColumn("dbo.Tickets", "Phone_AreaCode", c => c.Int());
            AddColumn("dbo.Tickets", "Phone_Extension", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Type", c => c.String());
            AddColumn("dbo.Tickets", "Phone_Carrier", c => c.String());
            AddColumn("dbo.Tickets", "ContactPreference", c => c.Int());
            AddColumn("dbo.Tickets", "TableRequest", c => c.String());
            AddColumn("dbo.Tickets", "NumberOfGuests", c => c.Int());
            AddColumn("dbo.Tickets", "Notes", c => c.String());
            AddColumn("dbo.Slots", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "ConceptCode", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Lat", c => c.Double(nullable: false));
            AddColumn("dbo.Stores", "Lon", c => c.Double(nullable: false));
            AddColumn("dbo.Stores", "LocationCode", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Occurrences", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Occurrences", "Discriminator", c => c.String(maxLength: 128));
            AddColumn("dbo.ResEvents", "Subhead", c => c.String());
            AddColumn("dbo.ResEvents", "Summary", c => c.String());
            AddColumn("dbo.ResEvents", "Over21", c => c.Boolean());
            AddColumn("dbo.ResEvents", "FeaturedEvent", c => c.Boolean());
            AddColumn("dbo.ResEvents", "image_MediaId", c => c.Int());
            AddColumn("dbo.ResEvents", "NprLocation_NPRLocationId", c => c.Int());
            AddColumn("dbo.ReservationTypes", "Url", c => c.String(nullable: false));
            AddColumn("dbo.GiveawayEvents", "MaxItems", c => c.Int(nullable: false));
            AddColumn("dbo.GiveawayEvents", "MinItems", c => c.Int(nullable: false));
            AddColumn("dbo.TourSlots", "GuideId", c => c.Int());
            AddColumn("dbo.ResUsers", "EmailInsiders", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ResEvents", "Url", c => c.String(nullable: false));
            AddForeignKey("dbo.ResEvents", "image_MediaId", "dbo.Media", "MediaId");
            AddForeignKey("dbo.ResEvents", "NprLocation_NPRLocationId", "dbo.NPRLocations", "NPRLocationId");
            AddForeignKey("dbo.TourSlots", "GuideId", "dbo.Users", "UserId");
            CreateIndex("dbo.ResEvents", "image_MediaId");
            CreateIndex("dbo.ResEvents", "NprLocation_NPRLocationId");
            CreateIndex("dbo.TourSlots", "GuideId");
            DropColumn("dbo.Slots", "StatusId");
            DropColumn("dbo.Stores", "ConceptCodeId");
            DropColumn("dbo.Stores", "LocationCodeId");
            DropColumn("dbo.Stores", "StatusId");
            DropColumn("dbo.Occurrences", "StatusId");
            DropColumn("dbo.ResEvents", "UrlName");
            DropColumn("dbo.ReservationTypes", "UrlName");
            DropColumn("dbo.TourSlots", "Guide_UserId");
            DropColumn("dbo.ResUsers", "TourTicket_TicketId");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId1");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId2");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId3");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId4");
            DropTable("dbo.CameoSets");
            DropTable("dbo.ResUserResStores");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ResUserResStores",
                c => new
                    {
                        ResUser_UserId = c.Int(nullable: false),
                        ResStore_LocationNumber = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ResUser_UserId, t.ResStore_LocationNumber });
            
            CreateTable(
                "dbo.CameoSets",
                c => new
                    {
                        CameoSetsId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CameoSetsId);
            
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId4", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId3", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId2", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId1", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "TourTicket_TicketId", c => c.Int());
            AddColumn("dbo.TourSlots", "Guide_UserId", c => c.Int());
            AddColumn("dbo.ReservationTypes", "UrlName", c => c.String(nullable: false));
            AddColumn("dbo.ResEvents", "UrlName", c => c.String(nullable: false));
            AddColumn("dbo.Occurrences", "StatusId", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "StatusId", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "LocationCodeId", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "ConceptCodeId", c => c.Int(nullable: false));
            AddColumn("dbo.Slots", "StatusId", c => c.Int(nullable: false));
            DropIndex("dbo.TourSlots", new[] { "GuideId" });
            DropIndex("dbo.Cameos", new[] { "TourSlotId" });
            DropIndex("dbo.Cameos", new[] { "ResUserId" });
            DropIndex("dbo.NPRLocations", new[] { "AddressId" });
            DropIndex("dbo.ResEvents", new[] { "NprLocation_NPRLocationId" });
            DropIndex("dbo.ResEvents", new[] { "image_MediaId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "UserId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "StoreId" });
            DropForeignKey("dbo.TourSlots", "GuideId", "dbo.Users");
            DropForeignKey("dbo.Cameos", "TourSlotId", "dbo.TourSlots");
            DropForeignKey("dbo.Cameos", "ResUserId", "dbo.ResUsers");
            DropForeignKey("dbo.NPRLocations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.ResEvents", "NprLocation_NPRLocationId", "dbo.NPRLocations");
            DropForeignKey("dbo.ResEvents", "image_MediaId", "dbo.Media");
            DropForeignKey("dbo.LocationSubscriptions", "UserId", "dbo.ResUsers");
            DropForeignKey("dbo.LocationSubscriptions", "StoreId", "dbo.Stores");
            AlterColumn("dbo.ResEvents", "Url", c => c.String());
            DropColumn("dbo.ResUsers", "EmailInsiders");
            DropColumn("dbo.TourSlots", "GuideId");
            DropColumn("dbo.GiveawayEvents", "MinItems");
            DropColumn("dbo.GiveawayEvents", "MaxItems");
            DropColumn("dbo.ReservationTypes", "Url");
            DropColumn("dbo.ResEvents", "NprLocation_NPRLocationId");
            DropColumn("dbo.ResEvents", "image_MediaId");
            DropColumn("dbo.ResEvents", "FeaturedEvent");
            DropColumn("dbo.ResEvents", "Over21");
            DropColumn("dbo.ResEvents", "Summary");
            DropColumn("dbo.ResEvents", "Subhead");
            DropColumn("dbo.Occurrences", "Discriminator");
            DropColumn("dbo.Occurrences", "Status");
            DropColumn("dbo.Stores", "Status");
            DropColumn("dbo.Stores", "LocationCode");
            DropColumn("dbo.Stores", "Lon");
            DropColumn("dbo.Stores", "Lat");
            DropColumn("dbo.Stores", "ConceptCode");
            DropColumn("dbo.Slots", "Status");
            DropColumn("dbo.Tickets", "Notes");
            DropColumn("dbo.Tickets", "NumberOfGuests");
            DropColumn("dbo.Tickets", "TableRequest");
            DropColumn("dbo.Tickets", "ContactPreference");
            DropColumn("dbo.Tickets", "Phone_Carrier");
            DropColumn("dbo.Tickets", "Phone_Type");
            DropColumn("dbo.Tickets", "Phone_Extension");
            DropColumn("dbo.Tickets", "Phone_AreaCode");
            DropColumn("dbo.Tickets", "Phone_Full");
            DropColumn("dbo.Tickets", "Phone_Number");
            DropColumn("dbo.Tickets", "EmailConfirm");
            DropColumn("dbo.Tickets", "Email");
            DropColumn("dbo.Tickets", "LastName");
            DropColumn("dbo.Tickets", "FirstName");
            DropTable("dbo.Cameos");
            DropTable("dbo.NPRLocations");
            DropTable("dbo.LocationSubscriptions");
            //RenameColumn(table: "dbo.Cameos", name: "TourSlotId", newName: "Cameos_CameoSetsId");
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirectorId", newName: "UnitMarketingDirector_UserId");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId4");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId3");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId2");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId1");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId");
            CreateIndex("dbo.ResUsers", "TourTicket_TicketId");
            CreateIndex("dbo.TourSlots", "Cameos_CameoSetsId");
            CreateIndex("dbo.TourSlots", "Guide_UserId");
            CreateIndex("dbo.ResUserResStores", "ResStore_LocationNumber");
            CreateIndex("dbo.ResUserResStores", "ResUser_UserId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets", "TicketId");
            AddForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores", "LocationNumber", cascadeDelete: true);
            AddForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers", "UserId", cascadeDelete: true);
        }
    }
}
