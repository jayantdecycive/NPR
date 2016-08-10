namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GiveawayOccurrences", "GiveawayOccurrenceID", c => c.Int(nullable: false));
        }
    }
}
