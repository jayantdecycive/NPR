namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ticket_no : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Media", "CreationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Media", "CreationDate", c => c.DateTime(nullable: false));
        }
    }
}