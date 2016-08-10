namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_item_change : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.Tickets", new[] { "Item_MenuItemId" });
            RenameColumn(table: "dbo.Tickets", name: "Item_MenuItemId", newName: "MenuItemId");
            AddColumn("dbo.Tickets", "Status", c => c.Int(nullable: false));
            AddForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            CreateIndex("dbo.Tickets", "MenuItemId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", new[] { "MenuItemId" });
            DropForeignKey("dbo.Tickets", "MenuItemId", "dbo.MenuItems");
            DropColumn("dbo.Tickets", "Status");
            RenameColumn(table: "dbo.Tickets", name: "MenuItemId", newName: "Item_MenuItemId");
            CreateIndex("dbo.Tickets", "Item_MenuItemId");
            AddForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems", "MenuItemId");
        }
    }
}
