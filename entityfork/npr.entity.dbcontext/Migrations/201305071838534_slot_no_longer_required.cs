namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slot_no_longer_required : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            AlterColumn("dbo.Tickets", "SlotId", c => c.Int());
            AddForeignKey("dbo.Tickets", "SlotId", "dbo.Slots", "SlotId");
            CreateIndex("dbo.Tickets", "SlotId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", new[] { "SlotId" });
            DropForeignKey("dbo.Tickets", "SlotId", "dbo.Slots");
            AlterColumn("dbo.Tickets", "SlotId", c => c.Int(nullable: false));
            CreateIndex("dbo.Tickets", "SlotId");
            AddForeignKey("dbo.Tickets", "SlotId", "dbo.Slots", "SlotId", cascadeDelete: true);
        }
    }
}
