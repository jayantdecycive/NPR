namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEvent1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "SiteStart", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResEvents", "SiteEnd", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "SiteEnd");
            DropColumn("dbo.ResEvents", "SiteStart");
        }
    }
}
