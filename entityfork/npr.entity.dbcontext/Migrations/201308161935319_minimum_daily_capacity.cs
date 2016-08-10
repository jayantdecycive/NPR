namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minimum_daily_capacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "OriginalCapacity", c => c.Int(nullable: false));
            AddColumn("dbo.ResEvents", "MaximumCapacity", c => c.Int());
            AddColumn("dbo.ResEvents", "MinimumDailyCapacity", c => c.Int(nullable: false));
            DropColumn("dbo.GiveawayOccurrences", "SampleProp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GiveawayOccurrences", "SampleProp", c => c.String());
            DropColumn("dbo.ResEvents", "MinimumDailyCapacity");
            DropColumn("dbo.ResEvents", "MaximumCapacity");
            DropColumn("dbo.Slots", "OriginalCapacity");
        }
    }
}
