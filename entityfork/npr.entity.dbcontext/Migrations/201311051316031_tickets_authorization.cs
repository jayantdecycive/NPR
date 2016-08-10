namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class tickets_authorization : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "IsPaid", c => c.Boolean(nullable: false, defaultValue: false));
			AddColumn("dbo.Tickets", "TicketAmount", c => c.Decimal(nullable: true, precision: 18, scale: 2));
			AddColumn("dbo.Tickets", "TotalAmount", c => c.Decimal(nullable: true, precision: 18, scale: 2));
			AddColumn("dbo.Tickets", "IsAuthSuccessful", c => c.Boolean(nullable: true, defaultValue: false));
			AddColumn("dbo.Tickets", "AuthorizationResponse", c => c.String(nullable: true, defaultValue: ""));
        }
        
        public override void Down()
        {
			DropColumn("dbo.Tickets", "IsPaid");
			DropColumn("dbo.Tickets", "TicketAmount");
			DropColumn("dbo.Tickets", "TotalAmount");
			DropColumn("dbo.Tickets", "IsAuthSuccessful");
			DropColumn("dbo.Tickets", "AuthorizationResponse");
        }
    }
}
