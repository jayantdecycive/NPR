namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class npr_clean : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LocationCategories",
                c => new
                    {
                        LocationCategoryId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        StrType = c.String(),
                    })
                .PrimaryKey(t => t.LocationCategoryId);
            
            AddColumn("dbo.ResEvents", "MustBeOfAgeToAttend", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasParking", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "IsCorporateOffice", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "MaximumCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Comments", c => c.String());
            AddColumn("dbo.Stores", "Category_LocationCategoryId", c => c.String(maxLength: 128));
            AddForeignKey("dbo.Stores", "Category_LocationCategoryId", "dbo.LocationCategories", "LocationCategoryId");
            CreateIndex("dbo.Stores", "Category_LocationCategoryId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Stores", new[] { "Category_LocationCategoryId" });
            DropForeignKey("dbo.Stores", "Category_LocationCategoryId", "dbo.LocationCategories");
            DropColumn("dbo.Stores", "Category_LocationCategoryId");
            DropColumn("dbo.Stores", "Comments");
            DropColumn("dbo.Stores", "MaximumCapacity");
            DropColumn("dbo.Stores", "IsCorporateOffice");
            DropColumn("dbo.Stores", "HasParking");
            DropColumn("dbo.ResEvents", "MustBeOfAgeToAttend");
            DropTable("dbo.LocationCategories");
        }
    }
}
