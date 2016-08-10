namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class template_media : DbMigration
    {
        public override void Up()
        {
            
            //DropForeignKey("dbo.ResTemplates", "FK_dbo.ResTemplates_dbo.Media_Icon_MediaId");
            //DropIndex("dbo.ResTemplates", new[] { "Icon_MediaId" });
            DropColumn("dbo.ResTemplates", "Icon_MediaId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResTemplates", "Icon_MediaId", c => c.Int());
            //CreateIndex("dbo.ResTemplates", "Icon_MediaId");
            //DropForeignKey("dbo.ResTemplates", "FK_dbo.ResTemplates_dbo.Media_Icon_MediaId");
            
        }
    }
}
