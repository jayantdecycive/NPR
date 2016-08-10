namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class guide_designation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers");
            DropForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users");
            DropIndex("dbo.ResUserResStores", new[] { "ResUser_UserId" });
            DropIndex("dbo.ResUserResStores", new[] { "ResStore_LocationNumber" });
            DropIndex("dbo.TourSlots", new[] { "Guide_UserId" });
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
            
            AddColumn("dbo.Tickets", "TableRequest", c => c.String());
            AddColumn("dbo.Tickets", "NumberOfGuests", c => c.Int());
            AddColumn("dbo.TourSlots", "GuideId", c => c.Int());
            AddColumn("dbo.ResUsers", "EmailInsiders", c => c.Boolean(nullable: false));
            AddForeignKey("dbo.TourSlots", "GuideId", "dbo.Users", "UserId");
            CreateIndex("dbo.TourSlots", "GuideId");
            DropColumn("dbo.TourSlots", "Guide_UserId");
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
            
            AddColumn("dbo.TourSlots", "Guide_UserId", c => c.Int());
            DropIndex("dbo.TourSlots", new[] { "GuideId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "UserId" });
            DropIndex("dbo.LocationSubscriptions", new[] { "StoreId" });
            DropForeignKey("dbo.TourSlots", "GuideId", "dbo.Users");
            DropForeignKey("dbo.LocationSubscriptions", "UserId", "dbo.ResUsers");
            DropForeignKey("dbo.LocationSubscriptions", "StoreId", "dbo.Stores");
            DropColumn("dbo.ResUsers", "EmailInsiders");
            DropColumn("dbo.TourSlots", "GuideId");
            DropColumn("dbo.Tickets", "NumberOfGuests");
            DropColumn("dbo.Tickets", "TableRequest");
            DropTable("dbo.LocationSubscriptions");
            CreateIndex("dbo.TourSlots", "Guide_UserId");
            CreateIndex("dbo.ResUserResStores", "ResStore_LocationNumber");
            CreateIndex("dbo.ResUserResStores", "ResUser_UserId");
            AddForeignKey("dbo.TourSlots", "Guide_UserId", "dbo.Users", "UserId");
            AddForeignKey("dbo.ResUserResStores", "ResStore_LocationNumber", "dbo.Stores", "LocationNumber", cascadeDelete: true);
            AddForeignKey("dbo.ResUserResStores", "ResUser_UserId", "dbo.ResUsers", "UserId", cascadeDelete: true);
        }
    }
}
