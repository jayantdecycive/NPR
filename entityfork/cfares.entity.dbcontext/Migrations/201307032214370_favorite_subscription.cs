namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class favorite_subscription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocationSubscriptions", "Favorite", c => c.Boolean(nullable: false,defaultValue:false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocationSubscriptions", "Favorite");
        }
    }
}
