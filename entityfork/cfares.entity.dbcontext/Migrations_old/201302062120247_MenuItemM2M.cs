namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuItemM2M : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropIndex("dbo.MenuItems", new[] { "GiveawayEvent_ResEventId" });
            CreateTable(
                "dbo.GiveawayEventMenuItems",
                c => new
                    {
                        GiveawayEvent_ResEventId = c.Int(nullable: false),
                        MenuItem_MenuItemId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GiveawayEvent_ResEventId, t.MenuItem_MenuItemId })
                .ForeignKey("dbo.GiveawayEvents", t => t.GiveawayEvent_ResEventId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuItem_MenuItemId, cascadeDelete: true)
                .Index(t => t.GiveawayEvent_ResEventId)
                .Index(t => t.MenuItem_MenuItemId);
            
            DropColumn("dbo.MenuItems", "GiveawayEvent_ResEventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "GiveawayEvent_ResEventId", c => c.Int());
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "MenuItem_MenuItemId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropForeignKey("dbo.GiveawayEventMenuItems", "MenuItem_MenuItemId", "dbo.MenuItems");
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropTable("dbo.GiveawayEventMenuItems");
            CreateIndex("dbo.MenuItems", "GiveawayEvent_ResEventId");
            AddForeignKey("dbo.MenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId");
        }
    }
}
