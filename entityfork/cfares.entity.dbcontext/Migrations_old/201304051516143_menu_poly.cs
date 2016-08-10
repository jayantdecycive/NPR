namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_poly : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiveawayEvents", "MaxItems", c => c.Int(nullable: false));
            AddColumn("dbo.GiveawayEvents", "MinItems", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GiveawayEvents", "MinItems");
            DropColumn("dbo.GiveawayEvents", "MaxItems");
        }
    }
}
