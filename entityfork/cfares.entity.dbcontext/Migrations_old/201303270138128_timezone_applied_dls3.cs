namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class timezone_applied_dls3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Slots", "Start", c => c.DateTimeOffset(nullable: false));
            AlterColumn("dbo.Slots", "Cutoff", c => c.DateTimeOffset(nullable: false));
            AlterColumn("dbo.Slots", "End", c => c.DateTimeOffset(nullable: false));
            DropColumn("dbo.Slots", "Availability_Start");
            DropColumn("dbo.Slots", "Availability_End");
            DropColumn("dbo.Slots", "Availability_Span");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Slots", "Availability_Span", c => c.Time(nullable: false));
            AddColumn("dbo.Slots", "Availability_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Slots", "Availability_Start", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Slots", "End", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Slots", "Cutoff", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Slots", "Start", c => c.DateTime(nullable: false));
        }
    }
}
