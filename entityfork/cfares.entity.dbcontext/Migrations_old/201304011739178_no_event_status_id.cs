namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class no_event_status_id : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ResEvents", "Status", c => c.Int(nullable: false));
            //DropColumn("dbo.ResEvents", "StatusId");
            RenameColumn("dbo.ResEvents", "StatusId", "Status");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.ResEvents", "Status", "StatusId");
            //AddColumn("dbo.ResEvents", "StatusId", c => c.Int(nullable: false));
            //DropColumn("dbo.ResEvents", "Status");
        }
    }
}
