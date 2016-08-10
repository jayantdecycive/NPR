namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeInTicketGuestRef1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketGuests", "Ticket_TicketId", "dbo.Tickets");
            DropIndex("dbo.TicketGuests", new[] { "Ticket_TicketId" });
            DropColumn("dbo.TicketGuests", "Ticket_TicketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketGuests", "Ticket_TicketId", c => c.Int());
            CreateIndex("dbo.TicketGuests", "Ticket_TicketId");
            AddForeignKey("dbo.TicketGuests", "Ticket_TicketId", "dbo.Tickets", "TicketId");
        }
    }
}
