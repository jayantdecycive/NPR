namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlotScheduleId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules");
            DropIndex("dbo.Slots", new[] { "ScheduleId" });
            RenameColumn(table: "dbo.Slots", name: "ScheduleId", newName: "Schedule_ScheduleId");
            AddForeignKey("dbo.Slots", "Schedule_ScheduleId", "dbo.Schedules", "ScheduleId");
            CreateIndex("dbo.Slots", "Schedule_ScheduleId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Slots", new[] { "Schedule_ScheduleId" });
            DropForeignKey("dbo.Slots", "Schedule_ScheduleId", "dbo.Schedules");
            RenameColumn(table: "dbo.Slots", name: "Schedule_ScheduleId", newName: "ScheduleId");
            CreateIndex("dbo.Slots", "ScheduleId");
            AddForeignKey("dbo.Slots", "ScheduleId", "dbo.Schedules", "ScheduleId", cascadeDelete: true);
        }
    }
}
