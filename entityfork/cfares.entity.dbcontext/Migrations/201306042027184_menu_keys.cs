namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_keys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            
            DropIndex("dbo.Tickets", new[] { "MenuItemId" });
            DropIndex("dbo.MenuItemAllowances", new[] { "AllowedItemId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_MenuItemId" });
            
            RenameColumn(table: "dbo.GiveawayOccurrenceMenuItems", name: "MenuItem_MenuItemId", newName: "MenuItem_DomId");
            RenameColumn(table: "dbo.GiveawaySlotMenuItems", name: "MenuItem_MenuItemId", newName: "MenuItem_DomId");
            
            DropPrimaryKey("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_DomId", "GiveawayOccurrence_OccurrenceId" });
            DropPrimaryKey("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_DomId", "GiveawaySlot_SlotId" });


            AlterColumn("dbo.GiveawayOccurrenceMenuItems", "MenuItem_DomId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.GiveawaySlotMenuItems", "MenuItem_DomId", c => c.String(nullable: false, maxLength: 128));

            AddPrimaryKey("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_DomId", "GiveawayOccurrence_OccurrenceId" });
            AddPrimaryKey("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_DomId", "GiveawaySlot_SlotId" });
            
            AlterColumn("dbo.MenuItems", "DomId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Tickets", "MenuItemId", c => c.String(maxLength: 128));

            DropPrimaryKey("dbo.MenuItemAllowances", new[] { "AllowedItemId", "ResEventId" });
            //DropColumn("dbo.MenuItemAllowances", "AllowedItemId");
            AlterColumn("dbo.MenuItemAllowances", "AllowedItemId", c => c.String(nullable: false, maxLength: 128));

            

            DropPrimaryKey("dbo.MenuItems", new[] { "MenuItemId" });
            AddPrimaryKey("dbo.MenuItems", "DomId");

            AddPrimaryKey("dbo.MenuItemAllowances", new[] { "AllowedItemId", "ResEventId" });
            
            AddForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems", "DomId", cascadeDelete: true);
            AddForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems", "DomId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_DomId", "dbo.MenuItems", "DomId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_DomId", "dbo.MenuItems", "DomId", cascadeDelete: true);
            CreateIndex("dbo.Tickets", "MenuItemId");
            CreateIndex("dbo.MenuItemAllowances", "AllowedItemId");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "MenuItem_DomId");
            CreateIndex("dbo.GiveawaySlotMenuItems", "MenuItem_DomId");
            DropColumn("dbo.MenuItems", "MenuItemId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "MenuItemId", c => c.Int(nullable: false, identity: true));
            DropIndex("dbo.GiveawaySlotMenuItems", new[] { "MenuItem_DomId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "MenuItem_DomId" });
            DropIndex("dbo.MenuItemAllowances", new[] { "AllowedItemId" });
            DropIndex("dbo.Tickets", new[] { "MenuItemId" });
            DropForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_DomId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_DomId", "dbo.MenuItems");
            DropForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems");
            DropForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems");
            DropPrimaryKey("dbo.MenuItems", new[] { "DomId" });
            AddPrimaryKey("dbo.MenuItems", "MenuItemId");
            AlterColumn("dbo.MenuItemAllowances", "AllowedItemId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tickets", "MenuItemId", c => c.Int());
            AlterColumn("dbo.MenuItems", "DomId", c => c.String());
            RenameColumn(table: "dbo.GiveawaySlotMenuItems", name: "MenuItem_DomId", newName: "MenuItem_MenuItemId");
            RenameColumn(table: "dbo.GiveawayOccurrenceMenuItems", name: "MenuItem_DomId", newName: "MenuItem_MenuItemId");
            CreateIndex("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId");
            CreateIndex("dbo.MenuItemAllowances", "AllowedItemId");
            CreateIndex("dbo.Tickets", "MenuItemId");
            AddForeignKey("dbo.GiveawaySlotMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
        }
    }
}
