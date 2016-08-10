namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class template_id : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ResEvents", name: "TemplateName", newName: "TemplateId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.ResEvents", name: "TemplateId", newName: "TemplateName");
        }
    }
}
