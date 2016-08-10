namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurrenceStores : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Occurrences", name: "Store_LocationNumber", newName: "StoreId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Occurrences", name: "StoreId", newName: "Store_LocationNumber");
        }
    }
}
