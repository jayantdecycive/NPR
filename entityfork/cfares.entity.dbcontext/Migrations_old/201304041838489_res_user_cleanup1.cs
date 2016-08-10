namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class res_user_cleanup1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets");
            DropIndex("dbo.ResUsers", new[] { "TourTicket_TicketId" });
            DropColumn("dbo.ResUsers", "TourTicket_TicketId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResUsers", "TourTicket_TicketId", c => c.Int());
            CreateIndex("dbo.ResUsers", "TourTicket_TicketId");
            AddForeignKey("dbo.ResUsers", "TourTicket_TicketId", "dbo.TourTickets", "TicketId");
        }
    }
}
