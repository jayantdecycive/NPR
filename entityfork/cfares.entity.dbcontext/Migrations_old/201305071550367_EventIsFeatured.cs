namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventIsFeatured : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "IsFeatured", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "IsFeatured");
        }
    }
}
