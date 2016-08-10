namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FoodSlotMenuItems",
                c => new
                    {
                        FoodSlot_SlotId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FoodSlot_SlotId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.FoodSlots", t => t.FoodSlot_SlotId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.FoodSlot_SlotId)
                .Index(t => t.MenuItem_MenuItemId);
            
            CreateTable(
                "dbo.GiveawayOccurrenceMenuItems",
                c => new
                    {
                        GiveawayOccurrence_OccurrenceId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawayOccurrence_OccurrenceId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.GiveawayOccurrences", t => t.GiveawayOccurrence_OccurrenceId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawayOccurrence_OccurrenceId)
                .Index(t => t.MenuItem_MenuItemId);
            
            CreateTable(
                "dbo.GiveawayOccurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceId)
                .Index(t => t.OccurrenceId);
            
            CreateTable(
                "dbo.FoodSlots",
                c => new
                    {
                        SlotId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .Index(t => t.SlotId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.FoodSlots", new[] { "SlotId" });
            DropIndex("dbo.GiveawayOccurrences", new[] { "OccurrenceId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.FoodSlotMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.FoodSlotMenuItems", new[] { "FoodSlot_SlotId" });
            DropForeignKey("dbo.FoodSlots", "SlotId", "dbo.Slots");
            DropForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences");
            DropForeignKey("dbo.FoodSlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.FoodSlotMenuItems", "FoodSlot_SlotId", "dbo.FoodSlots");
            DropTable("dbo.FoodSlots");
            DropTable("dbo.GiveawayOccurrences");
            DropTable("dbo.GiveawayOccurrenceMenuItems");
            DropTable("dbo.FoodSlotMenuItems");
        }
    }
}
