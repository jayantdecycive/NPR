namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class speakerevent4 : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE SpeakerEvents SET ScheduleId=1 WHERE ScheduleId=0");
            CreateIndex("dbo.SpeakerEvents", "ScheduleId");
            AddForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules", "ScheduleId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.SpeakerEvents", new[] { "ScheduleId" });
            DropForeignKey("dbo.SpeakerEvents", "ScheduleId", "dbo.Schedules");
        }
    }
}
