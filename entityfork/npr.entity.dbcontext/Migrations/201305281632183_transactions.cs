namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TicketTransactions",
                c => new
                    {
                        TicketTransactionId = c.String(nullable: false, maxLength: 128),
                        CreateDate = c.DateTime(nullable: false),
                        CustomerId = c.Int(),
                        Action = c.Int(nullable: false),
                        TicketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TicketTransactionId)
                .ForeignKey("dbo.Tickets", t => t.TicketId, cascadeDelete: true)
                .ForeignKey("dbo.ResUsers", t => t.CustomerId)
                .Index(t => t.TicketId)
                .Index(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketTransactions", new[] { "CustomerId" });
            DropIndex("dbo.TicketTransactions", new[] { "TicketId" });
            DropForeignKey("dbo.TicketTransactions", "CustomerId", "dbo.ResUsers");
            DropForeignKey("dbo.TicketTransactions", "TicketId", "dbo.Tickets");
            DropTable("dbo.TicketTransactions");
        }
    }
}
