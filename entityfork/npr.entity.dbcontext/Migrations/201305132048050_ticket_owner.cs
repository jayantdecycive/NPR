namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_owner : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tickets", "FirstName");
            DropColumn("dbo.Tickets", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "LastName", c => c.String());
            AddColumn("dbo.Tickets", "FirstName", c => c.String());
        }
    }
}
