namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GiveawayOccurrences", "SampleProp", c => c.String());
            DropColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.GiveawayOccurrences", "SampleProp");
        }
    }
}
