namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TicketName : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Tickets", "Slot_SlotId", "dbo.Slots");
            //DropIndex("dbo.Tickets", new[] { "Slot_SlotId" });
            AddColumn("dbo.TourTickets", "TourSlot_SlotId", c => c.Int());
            AddForeignKey("dbo.TourTickets", "TourSlot_SlotId", "dbo.TourSlots", "SlotId");
            CreateIndex("dbo.TourTickets", "TourSlot_SlotId");
            //DropColumn("dbo.Tickets", "Slot_SlotId");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Tickets", "Slot_SlotId", c => c.Int());
            DropIndex("dbo.TourTickets", new[] { "TourSlot_SlotId" });
            DropForeignKey("dbo.TourTickets", "TourSlot_SlotId", "dbo.TourSlots");
            DropColumn("dbo.TourTickets", "TourSlot_SlotId");
            //CreateIndex("dbo.Tickets", "Slot_SlotId");
            //AddForeignKey("dbo.Tickets", "Slot_SlotId", "dbo.Slots", "SlotId");
        }
    }
}
