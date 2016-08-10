namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FoodSlotMenuItems", "FoodSlot_SlotId", "dbo.FoodSlots");
            DropForeignKey("dbo.FoodSlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.FoodSlotMenuItems", new[] { "FoodSlot_SlotId" });
            DropIndex("dbo.FoodSlotMenuItems", new[] { "MenuItem_MenuItemId" });
            CreateTable(
                "dbo.GiveawaySlotMenuItems",
                c => new
                    {
                        GiveawaySlot_SlotId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawaySlot_SlotId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.FoodSlots", t => t.GiveawaySlot_SlotId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawaySlot_SlotId)
                .Index(t => t.MenuItem_MenuItemId);
            
            AddColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID", c => c.Int(nullable: false));
            DropTable("dbo.FoodSlotMenuItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.FoodSlotMenuItems",
                c => new
                    {
                        FoodSlot_SlotId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FoodSlot_SlotId, t.MenuItem_MenuItemId });
            
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "GiveawaySlot_SlotId" });
            DropForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "GiveawaySlot_SlotId", "dbo.FoodSlots");
            DropColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID");
            DropTable("dbo.GiveawaySlotMenuItems");
            CreateIndex("dbo.FoodSlotMenuItems", "MenuItem_MenuItemId");
            CreateIndex("dbo.FoodSlotMenuItems", "FoodSlot_SlotId");
            AddForeignKey("dbo.FoodSlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.FoodSlotMenuItems", "FoodSlot_SlotId", "dbo.FoodSlots", "SlotId", cascadeDelete: true);
        }
    }
}
