namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notes_etc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            AddColumn("dbo.Slots", "Notes", c => c.String());
            AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
            CreateIndex("dbo.Occurrences", "ResEventId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            DropColumn("dbo.Slots", "Notes");
            CreateIndex("dbo.Occurrences", "ResEventId");
            AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId");
        }
    }
}
