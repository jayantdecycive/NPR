namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class food_media : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "MediaId", c => c.Int());
            AddForeignKey("dbo.MenuItems", "MediaId", "dbo.Media", "MediaId");
            CreateIndex("dbo.MenuItems", "MediaId");
            DropColumn("dbo.MenuItems", "URLName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "URLName", c => c.String());
            DropIndex("dbo.MenuItems", new[] { "MediaId" });
            DropForeignKey("dbo.MenuItems", "MediaId", "dbo.Media");
            DropColumn("dbo.MenuItems", "MediaId");
        }
    }
}
