namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class single_template : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResEvents", "MobileTemplate_ResTemplateId", "dbo.ResTemplates");
            DropForeignKey("dbo.ResEvents", "TabletTemplate_ResTemplateId", "dbo.ResTemplates");
            DropIndex("dbo.ResEvents", new[] { "MobileTemplate_ResTemplateId" });
            DropIndex("dbo.ResEvents", new[] { "TabletTemplate_ResTemplateId" });
            DropColumn("dbo.ResEvents", "MobileTemplate_ResTemplateId");
            DropColumn("dbo.ResEvents", "TabletTemplate_ResTemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "TabletTemplate_ResTemplateId", c => c.String(maxLength: 128));
            AddColumn("dbo.ResEvents", "MobileTemplate_ResTemplateId", c => c.String(maxLength: 128));
            CreateIndex("dbo.ResEvents", "TabletTemplate_ResTemplateId");
            CreateIndex("dbo.ResEvents", "MobileTemplate_ResTemplateId");
            AddForeignKey("dbo.ResEvents", "TabletTemplate_ResTemplateId", "dbo.ResTemplates", "ResTemplateId");
            AddForeignKey("dbo.ResEvents", "MobileTemplate_ResTemplateId", "dbo.ResTemplates", "ResTemplateId");
        }
    }
}
