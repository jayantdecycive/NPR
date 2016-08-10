namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuItem2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "GiveawayEvent_ResEventId", c => c.Int());
            AddForeignKey("dbo.MenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId");
            CreateIndex("dbo.MenuItems", "GiveawayEvent_ResEventId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.MenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropForeignKey("dbo.MenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropColumn("dbo.MenuItems", "GiveawayEvent_ResEventId");
        }
    }
}
