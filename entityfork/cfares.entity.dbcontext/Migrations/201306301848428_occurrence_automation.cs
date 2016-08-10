namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_automation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "AutomaticallyEnableOccurrences", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "AutomaticallyEnableOccurrences");
        }
    }
}
