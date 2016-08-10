namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_change : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Stores", name: "Category_LocationCategoryId", newName: "CategoryId");
            DropColumn("dbo.LocationCategories", "StrType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LocationCategories", "StrType", c => c.String());
            RenameColumn(table: "dbo.Stores", name: "CategoryId", newName: "Category_LocationCategoryId");
        }
    }
}
