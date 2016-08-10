namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sync : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes");
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventTypeId" });
            AddForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes", "ReservationTypeId", cascadeDelete: true);
            AddForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
            CreateIndex("dbo.ResSiteUrls", "ResEventTypeId");
            CreateIndex("dbo.ResSiteUrls", "ResEventId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventTypeId" });
            DropForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes");
            CreateIndex("dbo.ResSiteUrls", "ResEventTypeId");
            CreateIndex("dbo.ResSiteUrls", "ResEventId");
            AddForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes", "ReservationTypeId");
            AddForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents", "ResEventId");
        }
    }
}
