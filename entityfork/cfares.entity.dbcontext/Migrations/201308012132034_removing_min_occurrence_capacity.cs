namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removing_min_occurrence_capacity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ResEvents", "MinOccurrenceCapacity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "MinOccurrenceCapacity", c => c.Int());
        }
    }
}
