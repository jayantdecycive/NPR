namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpeakerEvents", "ScheduleId", c => c.Int(nullable: false));
            //CreateIndex("dbo.SpeakerEvents", "ScheduleId");
            //AddForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules", "ScheduleId", cascadeDelete: true,name:"SpeakerEvents_FK_Schedule");

            AlterColumn("dbo.Schedules", "Start", c => c.DateTimeOffset(nullable: false));
            AlterColumn("dbo.Schedules", "End", c => c.DateTimeOffset(nullable: false));
            
            
            
            DropColumn("dbo.Schedules", "TimeRange_Start");
            DropColumn("dbo.Schedules", "TimeRange_End");
            DropColumn("dbo.Schedules", "TimeRange_Span");
            DropColumn("dbo.SpeakerEvents", "StartTimeTemplate");
            DropColumn("dbo.SpeakerEvents", "EndTimeTemplate");
            DropColumn("dbo.SpeakerEvents", "OffSiteCapacity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SpeakerEvents", "OffSiteCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.SpeakerEvents", "EndTimeTemplate", c => c.DateTimeOffset(nullable: false));
            AddColumn("dbo.SpeakerEvents", "StartTimeTemplate", c => c.DateTimeOffset(nullable: false));
            AddColumn("dbo.Schedules", "TimeRange_Span", c => c.Time(nullable: false));
            AddColumn("dbo.Schedules", "TimeRange_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Schedules", "TimeRange_Start", c => c.DateTime(nullable: false));
            //DropIndex("dbo.SpeakerEvents", new[] { "ScheduleId" });
            //DropForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules");
            AlterColumn("dbo.Schedules", "End", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Schedules", "Start", c => c.DateTime(nullable: false));
            DropColumn("dbo.SpeakerEvents", "ScheduleId");
        }
    }
}
