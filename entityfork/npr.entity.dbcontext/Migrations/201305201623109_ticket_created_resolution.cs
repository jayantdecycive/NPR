namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_created_resolution : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tickets", "Created");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Tickets", "Created", c => c.DateTime(nullable: false));
        }
    }
}
