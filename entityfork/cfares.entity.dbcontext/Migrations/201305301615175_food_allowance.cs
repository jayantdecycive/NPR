namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class food_allowance : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "MenuItem_MenuItemId" });
            CreateTable(
                "dbo.MenuItemAllowances",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                        AllowedItemId = c.Int(nullable: false),
                        Condition = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResEventId, t.AllowedItemId })
                .ForeignKey("dbo.GiveawayEvents", t => t.ResEventId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.AllowedItemId, cascadeDelete: true)
                .Index(t => t.ResEventId)
                .Index(t => t.AllowedItemId);
            
            CreateTable(
                "dbo.TicketTransactions",
                c => new
                    {
                        TicketTransactionId = c.String(nullable: false, maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        CustomerId = c.Int(),
                        Action = c.Int(nullable: false),
                        TicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TicketTransactionId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.CustomerId)
                .Index(t => t.TicketId)
                .Index(t => t.CustomerId);
            
            DropTable("dbo.GiveawayEventMenuItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GiveawayEventMenuItems",
                c => new
                    {
                        GiveawayEvent_ResEventId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawayEvent_ResEventId, t.MenuItem_MenuItemId });
            
            DropIndex("dbo.TicketTransactions", new[] { "CustomerId" });
            DropIndex("dbo.TicketTransactions", new[] { "TicketId" });
            DropIndex("dbo.MenuItemAllowances", new[] { "AllowedItemId" });
            DropIndex("dbo.MenuItemAllowances", new[] { "ResEventId" });
            DropForeignKey("dbo.TicketTransactions", "CustomerId", "dbo.ResUsers");
            DropForeignKey("dbo.TicketTransactions", "TicketId", "dbo.Tickets");
            DropForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems");
            DropForeignKey("dbo.MenuItemAllowances", "ResEventId", "dbo.GiveawayEvents");
            DropTable("dbo.TicketTransactions");
            DropTable("dbo.MenuItemAllowances");
            CreateIndex("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId");
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            AddForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId", cascadeDelete: true);
        }
    }
}
