namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _enum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Slots", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Status", c => c.Int(nullable: false));
            Sql("UPDATE Slots SET Status=StatusId");
            Sql("UPDATE Stores SET Status=StatusId");
            DropColumn("dbo.Slots", "StatusId");
            DropColumn("dbo.Stores", "StatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "StatusId", c => c.Int(nullable: false));
            AddColumn("dbo.Slots", "StatusId", c => c.Int(nullable: false));
            Sql("UPDATE Slots SET StatusId=Status");
            Sql("UPDATE Stores SET StatusId=Status");
            DropColumn("dbo.Stores", "Status");
            DropColumn("dbo.Slots", "Status");
        }
    }
}
