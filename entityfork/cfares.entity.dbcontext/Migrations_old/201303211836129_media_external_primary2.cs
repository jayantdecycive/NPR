namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_external_primary2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Media", "PK_dbo.Media");
            DropColumn("dbo.Media","MediaId",null);
            AddColumn("dbo.Media", "MediaId", c => c.Int(identity: true));
            AddPrimaryKey("dbo.Media", "MediaId", "PK_dbo.Media");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Media", "PK_dbo.Media");
            DropColumn("dbo.Media", "MediaId", null);
            AddColumn("dbo.Media", "MediaId", c => c.Int(identity: false));
            AddPrimaryKey("dbo.Media", "MediaId", "PK_dbo.Media");
        }
    }
}
