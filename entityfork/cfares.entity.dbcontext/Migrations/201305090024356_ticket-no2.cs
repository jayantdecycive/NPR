namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketno2 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tickets", "UIX_CardNumber");
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Tickets", "CardNumber", true, "UIX_CardNumber");
            
        }
    }
}
