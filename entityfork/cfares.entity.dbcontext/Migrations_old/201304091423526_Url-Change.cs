namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrlChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ResSiteUrls",
                c => new
                    {
                        SiteUrlId = c.Int(nullable: false, identity: true),
                        Url = c.String(nullable: false),
                        ResEventId = c.Int(),
                        ResEventTypeId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SiteUrlId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .ForeignKey("dbo.ReservationTypes", t => t.ResEventTypeId)
                .Index(t => t.ResEventId)
                .Index(t => t.ResEventTypeId);
            
            Sql("INSERT INTO ResSiteUrls (Url,ResEventId) (SELECT Url,ResEventId FROM ResEvents)");
            Sql("INSERT INTO ResSiteUrls (Url,ResEventTypeId) (SELECT Url,ReservationTypeId FROM ReservationTypes)");

            DropColumn("dbo.ResEvents", "Url");
            DropColumn("dbo.ReservationTypes", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationTypes", "Url", c => c.String(nullable: false));
            AddColumn("dbo.ResEvents", "Url", c => c.String(nullable: false));

            Sql("UPDATE ResEvents SET ResEvents.Url=ResSiteUrls.Url FROM ResEvents INNER JOIN ResSiteUrls on ResSiteUrls.ResEventId=ResEvents.ResEventId");
            Sql("UPDATE ReservationTypes SET ReservationTypes.Url=ResSiteUrls.Url FROM ReservationTypes INNER JOIN ResSiteUrls on ResSiteUrls.ResEventTypeId=ReservationTypes.ReservationTypeId");

            DropIndex("dbo.ResSiteUrls", new[] { "ResEventTypeId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventId" });
            DropForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes");
            DropForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents");
            DropTable("dbo.ResSiteUrls");
        }
    }
}
