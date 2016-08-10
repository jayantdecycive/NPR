namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SlotRemoveKeys1 : DbMigration
    {
        public override void Up()
        {
            /*DropForeignKey("dbo.Slots", "ResEvent_ResEventId", "dbo.ResEvents");
            DropIndex("dbo.Slots", new[] { "ResEvent_ResEventId" });
            DropColumn("dbo.Slots", "ResEvent_ResEventId");*/
        }
        
        public override void Down()
        {
            /*AddColumn("dbo.Slots", "ResEvent_ResEventId", c => c.Int());
            CreateIndex("dbo.Slots", "ResEvent_ResEventId");
            AddForeignKey("dbo.Slots", "ResEvent_ResEventId", "dbo.ResEvents", "ResEventId");*/
        }
    }
}
