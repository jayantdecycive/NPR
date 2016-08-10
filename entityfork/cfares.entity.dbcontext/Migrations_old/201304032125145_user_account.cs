namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_account : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [cfares].[dbo].[ResUsers] SET OperationRole=0");
            AlterColumn("dbo.ResUsers", "OperationRole", c => c.Int(false,false,0,"0"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResUsers", "OperationRole", c => c.Int());
        }
    }
}
