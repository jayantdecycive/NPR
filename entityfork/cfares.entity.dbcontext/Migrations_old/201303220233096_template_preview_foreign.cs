namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class template_preview_foreign : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.ResTemplates", "Preview_MediaId", "Media", "MediaId", false, "FK_dbo.ResTemplates_dbo.Media_Preview_MediaId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResTemplates", "FK_dbo.ResTemplates_dbo.Media_Preview_MediaId");
        }
    }
}
