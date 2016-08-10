namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class may17merge : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ResTemplates", name: "Preview_MediaId", newName: "PreviewId");
            AddColumn("dbo.ResEvents", "MediaId", c => c.Int());
            AddColumn("dbo.Slots", "Visibility", c => c.Int(nullable: false));
            AlterColumn("dbo.LocationCategories", "LocationCategoryId", c => c.String(nullable: false, maxLength: 128));
            AddForeignKey("dbo.ResEvents", "MediaId", "dbo.Media", "MediaId");
            CreateIndex("dbo.ResEvents", "MediaId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResEvents", new[] { "MediaId" });
            DropForeignKey("dbo.ResEvents", "MediaId", "dbo.Media");
            AlterColumn("dbo.LocationCategories", "LocationCategoryId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Slots", "Visibility");
            DropColumn("dbo.ResEvents", "MediaId");
            RenameColumn(table: "dbo.ResTemplates", name: "PreviewId", newName: "Preview_MediaId");
        }
    }
}
