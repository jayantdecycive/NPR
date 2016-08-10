namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UrlName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationTypes", "Url", c => c.String(nullable: false));
            AlterColumn("dbo.ResEvents", "Url", c => c.String(nullable: false));
            DropColumn("dbo.ResEvents", "UrlName");
            DropColumn("dbo.ReservationTypes", "UrlName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationTypes", "UrlName", c => c.String(nullable: false));
            AddColumn("dbo.ResEvents", "UrlName", c => c.String(nullable: false));
            AlterColumn("dbo.ResEvents", "Url", c => c.String());
            DropColumn("dbo.ReservationTypes", "Url");
        }
    }
}
