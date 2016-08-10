namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "Owner_UserId", "dbo.Users");
            DropForeignKey("dbo.Users", "TourTicket_TicketId", "dbo.TourTickets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.Users", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropIndex("dbo.Tickets", new[] { "Owner_UserId" });
            DropIndex("dbo.Users", new[] { "TourTicket_TicketId" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.Users", new[] { "CameoSets_CameoSetsId4" });
            CreateTable(
                "dbo.ResUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        TourTicket_TicketId = c.Int(),
                        CameoSets_CameoSetsId = c.String(maxLength: 128),
                        CameoSets_CameoSetsId1 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId2 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId3 = c.String(maxLength: 128),
                        CameoSets_CameoSetsId4 = c.String(maxLength: 128),
                        OperationRole = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .ForeignKey("dbo.TourTickets", t => t.TourTicket_TicketId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId1)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId2)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId3)
                .ForeignKey("dbo.CameoSets", t => t.CameoSets_CameoSetsId4)
                .Index(t => t.UserId)
                .Index(t => t.TourTicket_TicketId)
                .Index(t => t.CameoSets_CameoSetsId)
                .Index(t => t.CameoSets_CameoSetsId1)
                .Index(t => t.CameoSets_CameoSetsId2)
                .Index(t => t.CameoSets_CameoSetsId3)
                .Index(t => t.CameoSets_CameoSetsId4);
            
            AlterColumn("dbo.Users", "Discriminator", c => c.String(maxLength: 128));
            AddForeignKey("dbo.Tickets", "Owner_UserId", "dbo.ResUsers", "UserId");
            CreateIndex("dbo.Tickets", "Owner_UserId");
            DropColumn("dbo.Users", "OperationRole");
            DropColumn("dbo.Users", "TourTicket_TicketId");
            DropColumn("dbo.Users", "CameoSets_CameoSetsId");
            DropColumn("dbo.Users", "CameoSets_CameoSetsId1");
            DropColumn("dbo.Users", "CameoSets_CameoSetsId2");
            DropColumn("dbo.Users", "CameoSets_CameoSetsId3");
            DropColumn("dbo.Users", "CameoSets_CameoSetsId4");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CameoSets_CameoSetsId4", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "CameoSets_CameoSetsId3", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "CameoSets_CameoSetsId2", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "CameoSets_CameoSetsId1", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "CameoSets_CameoSetsId", c => c.String(maxLength: 128));
            AddColumn("dbo.Users", "TourTicket_TicketId", c => c.Int());
            AddColumn("dbo.Users", "OperationRole", c => c.Int());
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId4" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId3" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId2" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId1" });
            DropIndex("dbo.ResUsers", new[] { "CameoSets_CameoSetsId" });
            DropIndex("dbo.ResUsers", new[] { "TourTicket_TicketId" });
            DropIndex("dbo.ResUsers", new[] { "UserId" });
            DropIndex("dbo.Tickets", new[] { "Owner_UserId" });
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId4", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId3", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId2", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId1", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "CameoSets_CameoSetsId", "dbo.CameoSets");
            DropForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets");
            DropForeignKey("dbo.ResUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tickets", "Owner_UserId", "dbo.ResUsers");
            AlterColumn("dbo.Users", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.ResUsers");
            CreateIndex("dbo.Users", "CameoSets_CameoSetsId4");
            CreateIndex("dbo.Users", "CameoSets_CameoSetsId3");
            CreateIndex("dbo.Users", "CameoSets_CameoSetsId2");
            CreateIndex("dbo.Users", "CameoSets_CameoSetsId1");
            CreateIndex("dbo.Users", "CameoSets_CameoSetsId");
            CreateIndex("dbo.Users", "TourTicket_TicketId");
            CreateIndex("dbo.Tickets", "Owner_UserId");
            AddForeignKey("dbo.Users", "CameoSets_CameoSetsId4", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.Users", "CameoSets_CameoSetsId3", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.Users", "CameoSets_CameoSetsId2", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.Users", "CameoSets_CameoSetsId1", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.Users", "CameoSets_CameoSetsId", "dbo.CameoSets", "CameoSetsId");
            AddForeignKey("dbo.Users", "TourTicket_TicketId", "dbo.TourTickets", "TicketId");
            AddForeignKey("dbo.Tickets", "Owner_UserId", "dbo.Users", "UserId");
        }
    }
}
