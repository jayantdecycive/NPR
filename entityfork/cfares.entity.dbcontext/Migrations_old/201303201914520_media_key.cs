namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_key : DbMigration
    {
        public override void Up()
        {
            
            DropForeignKey("dbo.ResTemplates", "FK_dbo.ResTemplates_dbo.Media_Icon_MediaId");
            DropForeignKey("dbo.ResTemplates", "FK_dbo.ResTemplates_dbo.Media_Preview_MediaId");
            DropPrimaryKey("dbo.Media", new[] { "MediaId" });
            DropIndex("dbo.ResTemplates", "IX_Icon_MediaId");
            DropIndex("dbo.ResTemplates", "IX_Preview_MediaId");
            
            AlterColumn("dbo.ResTemplates", "Icon_MediaId", c => c.Int());
            AlterColumn("dbo.ResTemplates", "Preview_MediaId", c => c.Int());
            AlterColumn("dbo.Media", "MediaId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Media", "MediaId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ResTemplates", "Preview_MediaId", c => c.String(maxLength: 128));
            AlterColumn("dbo.ResTemplates", "Icon_MediaId", c => c.String(maxLength: 128));

            CreateIndex("dbo.ResTemplates", "Icon_MediaId", false, "IX_Icon_MediaId");
            CreateIndex("dbo.ResTemplates", "Preview_MediaId", false, "IX_Preview_MediaId");
            AddForeignKey("dbo.ResTemplates", "Icon_MediaId", "dbo.Media", "MediaId",false, "FK_dbo.ResTemplates_dbo.Media_Icon_MediaId");
            AddForeignKey("dbo.ResTemplates", "Preview_MediaId", "dbo.Media", "MediaId", false, "FK_dbo.ResTemplates_dbo.Media_Preview_MediaId");
            AddPrimaryKey("dbo.Media", new[] { "MediaId" });
        }
    }
}
