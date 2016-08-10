namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_visibility : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "Visibility", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "Visibility");
        }
    }
}
