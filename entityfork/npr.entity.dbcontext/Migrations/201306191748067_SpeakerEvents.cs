namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpeakerEvents : DbMigration
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
                        ScheduleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .ForeignKey("dbo.Addresses", t => t.OffSiteAddressId)
                .ForeignKey("dbo.Media", t => t.SpeakerMediaId)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .Index(t => t.ResEventId)
                .Index(t => t.OffSiteAddressId)
                .Index(t => t.SpeakerMediaId)
                .Index(t => t.ScheduleId);
            
            AddColumn("dbo.Occurrences", "OffSiteAddressId", c => c.Int());
            AddColumn("dbo.Occurrences", "OffSiteDescription", c => c.String());
            AlterColumn("dbo.Schedules", "Start", c => c.DateTimeOffset(nullable: false));
            AlterColumn("dbo.Schedules", "End", c => c.DateTimeOffset(nullable: false));
            AddForeignKey("dbo.Occurrences", "OffSiteAddressId", "dbo.Addresses", "AddressId");
            CreateIndex("dbo.Occurrences", "OffSiteAddressId");
            DropColumn("dbo.Schedules", "TimeRange_Start");
            DropColumn("dbo.Schedules", "TimeRange_End");
            DropColumn("dbo.Schedules", "TimeRange_Span");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "TimeRange_Span", c => c.Time(nullable: false));
            AddColumn("dbo.Schedules", "TimeRange_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Schedules", "TimeRange_Start", c => c.DateTime(nullable: false));
            DropIndex("dbo.SpeakerEvents", new[] { "ScheduleId" });
            DropIndex("dbo.SpeakerEvents", new[] { "SpeakerMediaId" });
            DropIndex("dbo.SpeakerEvents", new[] { "OffSiteAddressId" });
            DropIndex("dbo.SpeakerEvents", new[] { "ResEventId" });
            DropIndex("dbo.Occurrences", new[] { "OffSiteAddressId" });
            DropForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.SpeakerEvents", "SpeakerMediaId", "dbo.Media");
            DropForeignKey("dbo.SpeakerEvents", "OffSiteAddressId", "dbo.Addresses");
            DropForeignKey("dbo.SpeakerEvents", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.Occurrences", "OffSiteAddressId", "dbo.Addresses");
            AlterColumn("dbo.Schedules", "End", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Schedules", "Start", c => c.DateTime(nullable: false));
            DropColumn("dbo.Occurrences", "OffSiteDescription");
            DropColumn("dbo.Occurrences", "OffSiteAddressId");
            DropTable("dbo.SpeakerEvents");
        }
    }
}
