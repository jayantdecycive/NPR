namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class unique_url : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResSiteUrls", "Url", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.ResSiteUrls", "Url", true, "UIX_SiteUrl");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ResSiteUrls", "UIX_SiteUrl");
            AlterColumn("dbo.ResSiteUrls", "Url", c => c.String(nullable: false));
        }
    }
}
