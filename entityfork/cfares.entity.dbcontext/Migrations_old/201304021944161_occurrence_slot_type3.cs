namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences");
            DropForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences");
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.GiveawayEvents", new[] { "ResEventId" });
            DropIndex("dbo.GiveawayOccurrences", new[] { "OccurrenceId" });
            AddColumn("dbo.Occurrences", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId", cascadeDelete: true);
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId");
            DropTable("dbo.GiveawayEvents");
            DropTable("dbo.GiveawayOccurrences");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GiveawayOccurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false),
                        GiveawayOccurrenceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OccurrenceId);
            
            CreateTable(
                "dbo.GiveawayEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId);
            
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.ResEvents");
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(maxLength: 128));
            DropColumn("dbo.Occurrences", "Discriminator");
            CreateIndex("dbo.GiveawayOccurrences", "OccurrenceId");
            CreateIndex("dbo.GiveawayEvents", "ResEventId");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId");
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            AddForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences", "OccurrenceId");
            AddForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents", "ResEventId");
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences", "OccurrenceId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId", cascadeDelete: true);
        }
    }
}
