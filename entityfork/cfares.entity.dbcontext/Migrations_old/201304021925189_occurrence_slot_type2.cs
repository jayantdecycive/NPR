namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.FoodSlots");
            DropForeignKey("dbo.FoodSlots", "SlotId", "dbo.Slots");
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "GiveawaySlot_SlotId" });
            DropIndex("dbo.FoodSlots", new[] { "SlotId" });
            AddForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.Slots", "SlotId", cascadeDelete: true);
            CreateIndex("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId");
            DropTable("dbo.FoodSlots");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FoodSlots",
                c => new
                    {
                        SlotId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SlotId);
            
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "GiveawaySlot_SlotId" });
            DropForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.Slots");
            CreateIndex("dbo.FoodSlots", "SlotId");
            CreateIndex("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId");
            AddForeignKey("dbo.FoodSlots", "SlotId", "dbo.Slots", "SlotId");
            AddForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.FoodSlots", "SlotId", cascadeDelete: true);
        }
    }
}
