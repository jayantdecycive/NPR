namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Item_MenuItemId", c => c.Int());
            AddForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems", "MenuItemId");
            CreateIndex("dbo.Tickets", "Item_MenuItemId");
            DropColumn("dbo.TourSlots", "TourSlotId");
            DropColumn("dbo.TourTickets", "TourTicketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TourTickets", "TourTicketId", c => c.String());
            AddColumn("dbo.TourSlots", "TourSlotId", c => c.Int(nullable: false));
            DropIndex("dbo.Tickets", new[] { "Item_MenuItemId" });
            DropForeignKey("dbo.Tickets", "Item_MenuItemId", "dbo.MenuItems");
            DropColumn("dbo.Tickets", "Item_MenuItemId");
        }
    }
}
