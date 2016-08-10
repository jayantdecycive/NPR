namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mediarect : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "CropX", c => c.Int(nullable: false,defaultValue:0));
            AddColumn("dbo.Media", "CropY", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.Media", "CropWidth", c => c.Int(nullable: false, defaultValue: 0));
            AddColumn("dbo.Media", "CropHeight", c => c.Int(nullable: false, defaultValue: 0));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "CropHeight");
            DropColumn("dbo.Media", "CropWidth");
            DropColumn("dbo.Media", "CropY");
            DropColumn("dbo.Media", "CropX");
        }
    }
}
