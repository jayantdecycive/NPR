namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezone_applied_dls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "DayLightSavings", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "DayLightSavings");
        }
    }
}
