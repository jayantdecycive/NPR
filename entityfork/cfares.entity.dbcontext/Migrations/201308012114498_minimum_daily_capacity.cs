namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minimum_daily_capacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "MinimumDailyCapacity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "MinimumDailyCapacity");
        }
    }
}
