namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_range1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEvents", "ProtoOccurrence_OccurrenceId", "dbo.Occurrences");
            DropIndex("dbo.ResEvents", new[] { "ProtoOccurrence_OccurrenceId" });
            DropColumn("dbo.ResEvents", "ProtoOccurrence_OccurrenceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "ProtoOccurrence_OccurrenceId", c => c.Int());
            CreateIndex("dbo.ResEvents", "ProtoOccurrence_OccurrenceId");
            AddForeignKey("dbo.ResEvents", "ProtoOccurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId");
        }
    }
}
