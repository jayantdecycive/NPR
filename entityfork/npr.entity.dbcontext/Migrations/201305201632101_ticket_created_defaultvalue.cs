namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_created_defaultvalue : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tickets", "CreatedDate");
            AddColumn("dbo.Tickets", "CreatedDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "CreatedDate");
            AddColumn("dbo.Tickets", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
