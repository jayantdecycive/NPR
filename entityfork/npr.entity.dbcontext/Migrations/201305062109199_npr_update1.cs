namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class npr_update1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEvents", "image_MediaId", "dbo.Media");
            DropForeignKey("dbo.ResEvents", "NprLocation_NPRLocationId", "dbo.NPRLocations");
            DropForeignKey("dbo.NPRLocations", "AddressId", "dbo.Addresses");
            DropIndex("dbo.ResEvents", new[] { "image_MediaId" });
            DropIndex("dbo.ResEvents", new[] { "NprLocation_NPRLocationId" });
            DropIndex("dbo.NPRLocations", new[] { "AddressId" });
            CreateTable(
                "dbo.LocationCategories",
                c => new
                    {
                        LocationCategoryId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        StrType = c.String(),
                    })
                .PrimaryKey(t => t.LocationCategoryId);
            
            AddColumn("dbo.Tickets", "IsSpecialtyTicket", c => c.Boolean());
            AddColumn("dbo.LocationSubscriptions", "Role", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "AcceptsCfaCard", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasDiningRoom", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasDriveThru", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "OffersOnlineOrdering", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "OffersWireless", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "ServesBreakfast", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "ChildcareAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasParking", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "ParkingNotes", c => c.String());
            AddColumn("dbo.Stores", "IsCorporateOffice", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "MaximumCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Comments", c => c.String());
            AddColumn("dbo.Stores", "Category_LocationCategoryId", c => c.String(maxLength: 128));
            AddColumn("dbo.ResEvents", "MustBeOfAgeToAttend", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ResSiteUrls", "Url", c => c.String(nullable: false, maxLength: 200));
            AddForeignKey("dbo.Stores", "Category_LocationCategoryId", "dbo.LocationCategories", "LocationCategoryId");
            CreateIndex("dbo.Stores", "Category_LocationCategoryId");
            DropColumn("dbo.Slots", "TotalTickets");
            DropColumn("dbo.ResEvents", "Subhead");
            DropColumn("dbo.ResEvents", "Summary");
            DropColumn("dbo.ResEvents", "Over21");
            DropColumn("dbo.ResEvents", "FeaturedEvent");
            DropColumn("dbo.ResEvents", "image_MediaId");
            DropColumn("dbo.ResEvents", "NprLocation_NPRLocationId");
            DropTable("dbo.NPRLocations");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.NPRLocationId);
            
            AddColumn("dbo.ResEvents", "NprLocation_NPRLocationId", c => c.Int());
            AddColumn("dbo.ResEvents", "image_MediaId", c => c.Int());
            AddColumn("dbo.ResEvents", "FeaturedEvent", c => c.Boolean());
            AddColumn("dbo.ResEvents", "Over21", c => c.Boolean());
            AddColumn("dbo.ResEvents", "Summary", c => c.String());
            AddColumn("dbo.ResEvents", "Subhead", c => c.String());
            AddColumn("dbo.Slots", "TotalTickets", c => c.Int(nullable: false));
            DropIndex("dbo.Stores", new[] { "Category_LocationCategoryId" });
            DropForeignKey("dbo.Stores", "Category_LocationCategoryId", "dbo.LocationCategories");
            AlterColumn("dbo.ResSiteUrls", "Url", c => c.String(nullable: false));
            DropColumn("dbo.ResEvents", "MustBeOfAgeToAttend");
            DropColumn("dbo.Stores", "Category_LocationCategoryId");
            DropColumn("dbo.Stores", "Comments");
            DropColumn("dbo.Stores", "MaximumCapacity");
            DropColumn("dbo.Stores", "IsCorporateOffice");
            DropColumn("dbo.Stores", "ParkingNotes");
            DropColumn("dbo.Stores", "HasParking");
            DropColumn("dbo.Stores", "ChildcareAvailable");
            DropColumn("dbo.Stores", "ServesBreakfast");
            DropColumn("dbo.Stores", "OffersWireless");
            DropColumn("dbo.Stores", "OffersOnlineOrdering");
            DropColumn("dbo.Stores", "HasDriveThru");
            DropColumn("dbo.Stores", "HasDiningRoom");
            DropColumn("dbo.Stores", "AcceptsCfaCard");
            DropColumn("dbo.LocationSubscriptions", "Role");
            DropColumn("dbo.Tickets", "IsSpecialtyTicket");
            DropTable("dbo.LocationCategories");
            CreateIndex("dbo.NPRLocations", "AddressId");
            CreateIndex("dbo.ResEvents", "NprLocation_NPRLocationId");
            CreateIndex("dbo.ResEvents", "image_MediaId");
            AddForeignKey("dbo.NPRLocations", "AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.ResEvents", "NprLocation_NPRLocationId", "dbo.NPRLocations", "NPRLocationId");
            AddForeignKey("dbo.ResEvents", "image_MediaId", "dbo.Media", "MediaId");
        }
    }
}
