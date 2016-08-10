namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_table1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ResUsers", "OperationRole", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ResUsers", "OperationRole", c => c.Int(nullable: false));
        }
    }
}
