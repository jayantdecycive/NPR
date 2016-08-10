namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class giveaway : DbMigration
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
                .ForeignKey("dbo.MenuItems", t => t.AllowedItemId, cascadeDelete: true)
                .ForeignKey("dbo.GiveawayEvents", t => t.ResEventId, cascadeDelete: true)
                .Index(t => t.AllowedItemId)
                .Index(t => t.ResEventId);
            
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
            
            DropIndex("dbo.MenuItemAllowances", new[] { "ResEventId" });
            DropIndex("dbo.MenuItemAllowances", new[] { "AllowedItemId" });
            DropForeignKey("dbo.MenuItemAllowances", "ResEventId", "dbo.GiveawayEvents");
            DropForeignKey("dbo.MenuItemAllowances", "AllowedItemId", "dbo.MenuItems");
            DropTable("dbo.MenuItemAllowances");
            CreateIndex("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId");
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            AddForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems", "MenuItemId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId", cascadeDelete: true);
        }
    }
}
