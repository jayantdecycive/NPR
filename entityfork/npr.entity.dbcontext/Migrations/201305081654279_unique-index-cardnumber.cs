namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uniqueindexcardnumber : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tickets", "CardNumber", c => c.String(maxLength: 100));
            Sql("UPDATE Tickets SET CardNumber = NEWID()");
            CreateIndex("dbo.Tickets", "CardNumber", true, "UIX_CardNumber");
            AlterColumn("dbo.Tickets", "CardNumber", c => c.String(maxLength: 100, defaultValueSql: "NEWID()"));
            //, defaultValueSql: "NEWID()"
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tickets", "UIX_CardNumber");
            AlterColumn("dbo.Tickets", "CardNumber", c => c.String(nullable: false));
        }
    }
}
