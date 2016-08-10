namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurencesCapacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "AllocatedCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.ResEvents", "MinOccurrenceCapacity", c => c.Int());
            AddColumn("dbo.ResEvents", "AutomaticallyEnableOccurrences", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "AutomaticallyEnableOccurrences");
            DropColumn("dbo.ResEvents", "MinOccurrenceCapacity");
            DropColumn("dbo.Tickets", "AllocatedCapacity");
        }
    }
}
