namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurrenceStoreFix1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Slots", "OccurrenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Slots", "OccurrenceId", c => c.String());
        }
    }
}
