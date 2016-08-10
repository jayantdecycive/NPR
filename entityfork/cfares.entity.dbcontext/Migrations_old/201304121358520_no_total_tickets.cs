namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class no_total_tickets : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Slots", "TotalTickets");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Slots", "TotalTickets", c => c.Int(nullable: false));
        }
    }
}
