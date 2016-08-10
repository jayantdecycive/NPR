namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.SpeakerEvents", new[] { "ScheduleId" });
            AlterColumn("dbo.SpeakerEvents", "ScheduleId", c => c.Int(nullable: false));
            AddForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules", "ScheduleId", cascadeDelete: true);
            CreateIndex("dbo.SpeakerEvents", "ScheduleId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpeakerEvents", new[] { "ScheduleId" });
            DropForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules");
            AlterColumn("dbo.SpeakerEvents", "ScheduleId", c => c.Int());
            CreateIndex("dbo.SpeakerEvents", "ScheduleId");
            AddForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules", "ScheduleId");
        }
    }
}