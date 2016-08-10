namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class food_media1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MenuItems", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MenuItems", "ImageUrl", c => c.String());
        }
    }
}
