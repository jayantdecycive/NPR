namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class tickets_additional_info : DbMigration
    {
        public override void Up()
        {
			AddColumn("dbo.Tickets", "ListenToNprStation", c => c.String(nullable: true));
			AddColumn("dbo.Tickets", "VisitorOfWebsite", c => c.Boolean(nullable: false, defaultValue: false));
			AddColumn("dbo.Tickets", "Age", c => c.String(nullable: true));
			AddColumn("dbo.Tickets", "Race", c => c.String(nullable: true));
			AddColumn("dbo.Tickets", "TopicsOfInterest", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
			DropColumn("dbo.Tickets", "ListenToNprStation");
			DropColumn("dbo.Tickets", "VisitorOfWebsite");
			DropColumn("dbo.Tickets", "Age");
			DropColumn("dbo.Tickets", "Race");
			DropColumn("dbo.Tickets", "TopicsOfInterest");
        }
    }
}
