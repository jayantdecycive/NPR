namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_media : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "MediaId", c => c.Int());
            //AddColumn("dbo.Media", "CreatedDate", c => c.DateTime(nullable: false,defaultValueSql:"GETDATE()"));
            AddColumn("dbo.Media", "CropX", c => c.Int(nullable: false));
            AddColumn("dbo.Media", "CropY", c => c.Int(nullable: false));
            AddColumn("dbo.Media", "CropWidth", c => c.Int(nullable: false));
            AddColumn("dbo.Media", "CropHeight", c => c.Int(nullable: false));
            AlterColumn("dbo.LocationCategories", "LocationCategoryId", c => c.String(nullable: false, maxLength: 128));
            AddForeignKey("dbo.ResEvents", "MediaId", "dbo.Media", "MediaId");
            CreateIndex("dbo.ResEvents", "MediaId");
            //DropColumn("dbo.Media", "CreationDate");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Media", "CreationDate", c => c.DateTime(nullable: false));
            DropIndex("dbo.ResEvents", new[] { "MediaId" });
            DropForeignKey("dbo.ResEvents", "MediaId", "dbo.Media");
            AlterColumn("dbo.LocationCategories", "LocationCategoryId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Media", "CropHeight");
            DropColumn("dbo.Media", "CropWidth");
            DropColumn("dbo.Media", "CropY");
            DropColumn("dbo.Media", "CropX");
            //DropColumn("dbo.Media", "CreatedDate");
            DropColumn("dbo.ResEvents", "MediaId");
        }
    }
}
