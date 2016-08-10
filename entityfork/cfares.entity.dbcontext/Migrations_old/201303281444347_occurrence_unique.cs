namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_unique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Occurrences", new string[] { "StoreId","ResEventId"}, true,"IX_Occurrence_Unique_Store_Event");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Occurrences", "IX_Occurrence_Unique_Store_Event");
        }
    }
}
