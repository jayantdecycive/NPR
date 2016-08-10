namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_m2m : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEventResStores", "ResEvent_ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResEventResStores", "ResStore_LocationNumber", "dbo.Stores");
            DropIndex("dbo.ResEventResStores", new[] { "ResEvent_ResEventId" });
            DropIndex("dbo.ResEventResStores", new[] { "ResStore_LocationNumber" });
            DropTable("dbo.ResEventResStores");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ResEventResStores",
                c => new
                    {
                        ResEvent_ResEventId = c.Int(nullable: false),
                        ResStore_LocationNumber = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ResEvent_ResEventId, t.ResStore_LocationNumber });
            
            CreateIndex("dbo.ResEventResStores", "ResStore_LocationNumber");
            CreateIndex("dbo.ResEventResStores", "ResEvent_ResEventId");
            AddForeignKey("dbo.ResEventResStores", "ResStore_LocationNumber", "dbo.Stores", "LocationNumber", cascadeDelete: true);
            AddForeignKey("dbo.ResEventResStores", "ResEvent_ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
        }
    }
}
