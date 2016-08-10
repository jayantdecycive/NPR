namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hotfix_min_occurrence_capacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "MinOccurrenceCapacity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "MinOccurrenceCapacity");
        }
    }
}
