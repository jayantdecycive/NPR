namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_external_primary : DbMigration
    {
        public override void Up()
        {
            AddPrimaryKey("dbo.Media", "MediaId");
            
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Media", "MediaId");
        }
    }
}
