namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurrenceStoreFix : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Occurrences", "StoreId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Occurrences", "StoreId", c => c.String());
        }
    }
}
