namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketno3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Tickets", "CardNumber");
            AddColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: true, defaultValueSql: "NEWID()", maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "CardNumber");
            AddColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: false, maxLength: 100, defaultValueSql: "NEWID()"));
        }
    }
}
