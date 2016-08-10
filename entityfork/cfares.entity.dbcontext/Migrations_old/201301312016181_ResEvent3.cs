namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEvent3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ResEvents", "TemplateName");
            DropColumn("dbo.ResEvents", "MobileTemplateName");
            DropColumn("dbo.ResEvents", "TabletTemplateName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "TabletTemplateName", c => c.String());
            AddColumn("dbo.ResEvents", "MobileTemplateName", c => c.String());
            AddColumn("dbo.ResEvents", "TemplateName", c => c.String());
        }
    }
}
