namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezone_applied_dls1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "GMTOffsetTimeZone", c => c.DateTimeOffset(nullable: false));
            DropColumn("dbo.Stores", "GMTOffsetApplied");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "GMTOffsetApplied", c => c.Int(nullable: false));
            DropColumn("dbo.Stores", "GMTOffsetTimeZone");
        }
    }
}
