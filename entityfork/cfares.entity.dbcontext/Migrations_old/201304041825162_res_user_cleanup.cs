namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class res_user_cleanup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropIndex("dbo.TourSlots", new[] { "Cameos_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId4" });
            //RenameColumn(table: "dbo.Cameos", name: "Cameos_CameoSetsId", newName: "TourSlotId");
            //DropColumn(table: "dbo.CameoSets", name: "CameosSetsId");//, newName: "TourSlotId");
            CreateTable(
                "dbo.Cameos",
                c => new
                    {
                        ResUserId = c.Int(nullable: false),
                        TourSlotId = c.Int(nullable: false),
                        CameoType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ResUserId, t.TourSlotId })
                .ForeignKey("dbo.ResUsers", t => t.ResUserId, cascadeDelete: true)
                .ForeignKey("dbo.TourSlots", t => t.TourSlotId, cascadeDelete: true)
                .Index(t => t.ResUserId)
                .Index(t => t.TourSlotId);
            
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId1");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId2");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId3");
            DropColumn("dbo.ResUsers", "CameoSets_CameoSetsId4");
            //AddColumn(table: "dbo.Cameos", name: "TourSlotId",);
            DropTable("dbo.CameoSets");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CameoSets",
                c => new
                    {
                        CameoSetsId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.CameoSetsId);
            
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId4", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId3", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId2", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId1", c => c.String(maxLength: 128));
            AddColumn("dbo.ResUsers", "CameoSets_CameoSetsId", c => c.String(maxLength: 128));
            DropIndex("dbo.Cameos", new[] { "TourSlotId" });
            DropIndex("dbo.Cameos", new[] { "ResUserId" });
            DropForeignKey("dbo.Cameos", "TourSlotId", "dbo.TourSlots");
            DropForeignKey("dbo.Cameos", "ResUserId", "dbo.ResUsers");
            DropTable("dbo.Cameos");
            
            //AddColumn("dbo.CameoSets", "CameosSetsId", c => c.Int(true));
            
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId4");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId3");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId2");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId1");
            CreateIndex("dbo.ResUsers", "CameoSets_CameoSetsId");
            CreateIndex("dbo.TourSlots", "Cameos_CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.TourSlots", "Cameos_CameoSetsId", "dbo.CameoSets", "CameoSetsId");
        }
    }
}
