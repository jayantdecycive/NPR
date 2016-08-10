namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketguest1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketGuests", "TicketGuest_TicketGuestsId", "dbo.TicketGuests");
            DropIndex("dbo.TicketGuests", new[] { "TicketGuest_TicketGuestsId" });
            DropColumn("dbo.TicketGuests", "TicketGuest_TicketGuestsId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TicketGuests", "TicketGuest_TicketGuestsId", c => c.Int());
            CreateIndex("dbo.TicketGuests", "TicketGuest_TicketGuestsId");
            AddForeignKey("dbo.TicketGuests", "TicketGuest_TicketGuestsId", "dbo.TicketGuests", "TicketGuestsId");
        }
    }
}
