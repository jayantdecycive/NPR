namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_fk : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Occurrences", "ResEvent_ResEventId", "dbo.ResEvents");
            //DropIndex("dbo.Occurrences", new[] { "ResEvent_ResEventId" });
            //DropColumn("dbo.Occurrences", "ResEvent_ResEventId");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Occurrences", "ResEvent_ResEventId", c => c.Int());
            //CreateIndex("dbo.Occurrences", "ResEvent_ResEventId");
            //AddForeignKey("dbo.Occurrences", "ResEvent_ResEventId", "dbo.ResEvents", "ResEventId");
        }
    }
}
