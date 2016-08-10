namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeInTicketGuestRef : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets");
            DropIndex("dbo.TicketGuests", new[] { "TicketId" });
            AddColumn("dbo.TicketGuests", "Ticket_TicketId", c => c.Int());
            AlterColumn("dbo.TicketGuests", "TicketId", c => c.Int(nullable: false));
            AddForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets", "TicketId", cascadeDelete: true);
            AddForeignKey("dbo.TicketGuests", "Ticket_TicketId", "dbo.Tickets", "TicketId");
            CreateIndex("dbo.TicketGuests", "TicketId");
            CreateIndex("dbo.TicketGuests", "Ticket_TicketId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.TicketGuests", new[] { "Ticket_TicketId" });
            DropIndex("dbo.TicketGuests", new[] { "TicketId" });
            DropForeignKey("dbo.TicketGuests", "Ticket_TicketId", "dbo.Tickets");
            DropForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets");
            AlterColumn("dbo.TicketGuests", "TicketId", c => c.Int());
            DropColumn("dbo.TicketGuests", "Ticket_TicketId");
            CreateIndex("dbo.TicketGuests", "TicketId");
            AddForeignKey("dbo.TicketGuests", "TicketId", "dbo.Tickets", "TicketId");
        }
    }
}
