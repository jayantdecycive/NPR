namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SpeakerEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                        OffSiteAddressId = c.Int(),
                        OffSiteDescription = c.String(),
                        OffSiteParkingDescription = c.String(),
                        SpeakerName = c.String(),
                        SpeakerMediaId = c.Int(),
                        HasChildCare = c.Boolean(nullable: false),
                        OffSiteCapacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .ForeignKey("dbo.Addresses", t => t.OffSiteAddressId)
                .ForeignKey("dbo.Media", t => t.SpeakerMediaId)
                .Index(t => t.ResEventId)
                .Index(t => t.OffSiteAddressId)
                .Index(t => t.SpeakerMediaId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpeakerEvents", new[] { "SpeakerMediaId" });
            DropIndex("dbo.SpeakerEvents", new[] { "OffSiteAddressId" });
            DropIndex("dbo.SpeakerEvents", new[] { "ResEventId" });
            DropForeignKey("dbo.SpeakerEvents", "SpeakerMediaId", "dbo.Media");
            DropForeignKey("dbo.SpeakerEvents", "OffSiteAddressId", "dbo.Addresses");
            DropForeignKey("dbo.SpeakerEvents", "ResEventId", "dbo.ResEvents");
            DropTable("dbo.SpeakerEvents");
        }
    }
}
