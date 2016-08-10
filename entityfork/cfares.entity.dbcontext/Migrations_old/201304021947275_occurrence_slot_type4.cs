namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_slot_type4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.Occurrences");
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            CreateTable(
                "dbo.GiveawayEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .Index(t => t.ResEventId);
            
            CreateTable(
                "dbo.GiveawayOccurrences",
                c => new
                    {
                        OccurrenceId = c.Int(nullable: false),
                        GiveawayOccurrenceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OccurrenceId)
                .ForeignKey("dbo.Occurrences", t => t.OccurrenceId)
                .Index(t => t.OccurrenceId);
            
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(maxLength: 128));
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents", "ResEventId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences", "OccurrenceId", cascadeDelete: true);
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId");
            DropColumn("dbo.Occurrences", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Occurrences", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropIndex("dbo.GiveawayOccurrences", new[] { "OccurrenceId" });
            DropIndex("dbo.GiveawayEvents", new[] { "ResEventId" });
            DropIndex("dbo.GiveawayOccurrenceMenuItems", new[] { "GiveawayOccurrence_OccurrenceId" });
            DropIndex("dbo.GiveawayEventMenuItems", new[] { "GiveawayEvent_ResEventId" });
            DropForeignKey("dbo.GiveawayOccurrences", "OccurrenceId", "dbo.Occurrences");
            DropForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents");
            DropForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.GiveawayOccurrences");
            DropForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.GiveawayEvents");
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.GiveawayOccurrences");
            DropTable("dbo.GiveawayEvents");
            CreateIndex("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId");
            CreateIndex("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId");
            AddForeignKey("dbo.GiveawayOccurrenceMenuItems", "GiveawayOccurrence_OccurrenceId", "dbo.Occurrences", "OccurrenceId", cascadeDelete: true);
            AddForeignKey("dbo.GiveawayEventMenuItems", "GiveawayEvent_ResEventId", "dbo.ResEvents", "ResEventId", cascadeDelete: true);
        }
    }
}
