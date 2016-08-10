namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticketno : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Tickets","UIX_CardNumber");
            DropColumn("dbo.Tickets", "CardNumber");
            AddColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: false, defaultValueSql: "NEWID()",maxLength:100));
            Sql("UPDATE Tickets SET CardNumber=NEWID()");
            CreateIndex("dbo.Tickets","CardNumber",true, "UIX_CardNumber");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "CardNumber");
            AddColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: true, maxLength: 100));
        }
    }
}
