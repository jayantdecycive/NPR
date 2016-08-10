namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transcations2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketTransactions", "TicketGuestsId", c => c.Int());
            AddForeignKey("dbo.TicketTransactions", "TicketGuestsId", "dbo.TicketGuests", "TicketGuestsId");
            CreateIndex("dbo.TicketTransactions", "TicketGuestsId");
        }
        
        public override void Down()
        {           
            DropIndex("dbo.TicketTransactions", new[] { "TicketGuestsId" });
            DropForeignKey("dbo.TicketTransactions", "TicketGuestsId", "dbo.TicketGuests");
            DropColumn("dbo.TicketTransactions", "TicketGuestsId");
        }
    }
}
