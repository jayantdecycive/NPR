namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MenuItem_MG : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GiveawayEvents",
                c => new
                    {
                        ResEventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResEventId)
                .ForeignKey("dbo.ResEvents", t => t.ResEventId)
                .Index(t => t.ResEventId);
            
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            DropIndex("dbo.GiveawayEvents", new[] { "ResEventId" });
            DropForeignKey("dbo.GiveawayEvents", "ResEventId", "dbo.ResEvents");
            AlterColumn("dbo.ResEvents", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.GiveawayEvents");
        }
    }
}
