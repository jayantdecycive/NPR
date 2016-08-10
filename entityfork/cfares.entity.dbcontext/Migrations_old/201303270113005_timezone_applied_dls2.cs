namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezone_applied_dls2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "GMTOffsetApplied", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "GMTOffsetApplied");
        }
    }
}
