namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class transactions1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketTransactions", "TicketTransactionId", c => c.String(nullable: false,maxLength:128,defaultValueSql:"NEWID()"));
            //DropColumn("dbo.TicketTransactions", "CreateDate");
            AlterColumn("dbo.TicketTransactions", "CreateDate", c => c.DateTime(nullable: false, defaultValueSql: "GETDATE()"));
        }
        
        public override void Down()
        {
        }
    }
}
