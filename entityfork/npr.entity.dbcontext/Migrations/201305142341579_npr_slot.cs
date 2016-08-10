namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class npr_slot : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NPRSlots",
                c => new
                    {
                        SlotId = c.Int(nullable: false),
                        GuideId = c.Int(),
                    })
                .PrimaryKey(t => t.SlotId)
                .ForeignKey("dbo.Slots", t => t.SlotId)
                .Index(t => t.SlotId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.NPRSlots", new[] { "SlotId" });
            DropForeignKey("dbo.NPRSlots", "SlotId", "dbo.Slots");
            DropTable("dbo.NPRSlots");
        }
    }
}
