namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_created : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "Created");
        }
    }
}
