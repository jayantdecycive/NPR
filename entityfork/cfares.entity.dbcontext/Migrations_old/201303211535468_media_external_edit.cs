namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_external_edit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Media", "ExternalUriStr", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Media", "ExternalUriStr");
        }
    }
}
