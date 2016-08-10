namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketcardnumberstring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tickets", "CardNumber", c => c.Int(nullable: false));
        }
    }
}
