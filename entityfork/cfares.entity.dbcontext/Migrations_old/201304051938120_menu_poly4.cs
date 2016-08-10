namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menu_poly4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Occurrences", "Status", c => c.Int(nullable: false));
            Sql("UPDATE Occurrences SET Status=StatusId");
            DropColumn("dbo.Occurrences", "StatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Occurrences", "StatusId", c => c.Int(nullable: false));
            Sql("UPDATE Occurrences SET StatusId=Status");
            DropColumn("dbo.Occurrences", "Status");
        }
    }
}
