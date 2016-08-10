namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class npr_update : DbMigration
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
                .ForeignKey("dbo.ReservationTypes", t => t.ResEventTypeId, cascadeDelete: true)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId, cascadeDelete: true)
                .Index(t => t.ResEventTypeId)
                .Index(t => t.ResEventId);
            
            DropColumn("dbo.ResEvents", "Url");
            DropColumn("dbo.ReservationTypes", "Url");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationTypes", "Url", c => c.String(nullable: false));
            AddColumn("dbo.ResEvents", "Url", c => c.String(nullable: false));
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventId" });
            DropIndex("dbo.ResSiteUrls", new[] { "ResEventTypeId" });
            DropForeignKey("dbo.ResSiteUrls", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.ResSiteUrls", "ResEventTypeId", "dbo.ReservationTypes");
            DropTable("dbo.ResSiteUrls");
        }
    }
}
