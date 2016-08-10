namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlotRemoveKeys : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Slots", "IX_ResEvent_ResEventId");
            DropForeignKey("dbo.Slots", "FK_dbo.Slots_dbo.ResEvents_ResEvent_ResEventId");
            DropColumn("dbo.Slots", "ResEvent_ResEventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Slots", "ResEvent_ResEventId", c => c.Int());
            CreateIndex("dbo.Slots", "ResEvent_ResEventId");
            AddForeignKey("dbo.Slots", "ResEvent_ResEventId", "dbo.ResEvents", "ResEventId");
        }
    }
}
