namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class restemplatemedia : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.ResTemplates", name: "Preview_MediaId", newName: "PreviewId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.ResTemplates", name: "PreviewId", newName: "Preview_MediaId");
        }
    }
}
