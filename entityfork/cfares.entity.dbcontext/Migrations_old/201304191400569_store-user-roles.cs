namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class storeuserroles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LocationSubscriptions", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LocationSubscriptions", "Role");
        }
    }
}
