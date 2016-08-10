namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEventUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "Url");
        }
    }
}
