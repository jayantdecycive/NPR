namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_subheading : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "SubHeading", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "SubHeading");
        }
    }
}
