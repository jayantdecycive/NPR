namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class tickets_cc_info : DbMigration
    {
        public override void Up()
        {
			AddColumn("dbo.Tickets", "CCNumber", c => c.String(nullable: true));
			AddColumn("dbo.Tickets", "CCExpDateMonth", c => c.Int(nullable: true));
			AddColumn("dbo.Tickets", "CCExpDateYear", c => c.Int(nullable: true));
			AddColumn("dbo.Tickets", "CCName", c => c.String(nullable: true));
			AddColumn("dbo.Tickets", "CCType", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
			DropColumn("dbo.Tickets", "CCNumber");
			DropColumn("dbo.Tickets", "CCExpDateMonth");
			DropColumn("dbo.Tickets", "CCExpDateYear");
			DropColumn("dbo.Tickets", "CCName");
			DropColumn("dbo.Tickets", "CCType");
        }
    }
}
