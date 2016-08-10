namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_region : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stores", "OperatorTeamName", c => c.String());
            AlterColumn("dbo.Stores", "RegionName", c => c.String());
            AlterColumn("dbo.Stores", "ServiceTeamName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stores", "ServiceTeamName", c => c.String(nullable: false));
            AlterColumn("dbo.Stores", "RegionName", c => c.String(nullable: false));
            AlterColumn("dbo.Stores", "OperatorTeamName", c => c.String(nullable: false));
        }
    }
}
