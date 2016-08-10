namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cascade : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
            CreateIndex("dbo.Occurrences", "ResEventId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Occurrences", new[] { "ResEventId" });
            DropForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents");
            CreateIndex("dbo.Occurrences", "ResEventId");
            AddForeignKey("dbo.Occurrences", "ResEventId", "dbo.ResEvents", "ResEventId");
        }
    }
}
