namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class resevent_paid_tickets : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "IsPaid", c => c.Boolean(nullable: false, defaultValue: false));
			AddColumn("dbo.ResEvents", "TicketAmount", c => c.Decimal(nullable: true, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
			DropColumn("dbo.ResEvents", "IsPaid");
			DropColumn("dbo.ResEvents", "TicketAmount");
        }
    }
}
