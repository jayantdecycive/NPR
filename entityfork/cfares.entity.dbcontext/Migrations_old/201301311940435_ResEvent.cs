namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEvent_migration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "RegistrationStart", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResEvents", "RegistrationEnd", c => c.DateTime(nullable: false));
            AddColumn("dbo.Users", "AccountStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "OperationRole", c => c.Int());
            AddColumn("dbo.Media", "MediaType", c => c.Int(nullable: false));
            DropColumn("dbo.ResEvents", "RegistrationAvailability_Start");
            DropColumn("dbo.ResEvents", "RegistrationAvailability_End");
            DropColumn("dbo.ResEvents", "RegistrationAvailability_Span");
            DropColumn("dbo.ResEvents", "SiteAvailability_Start");
            DropColumn("dbo.ResEvents", "SiteAvailability_End");
            DropColumn("dbo.ResEvents", "SiteAvailability_Span");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "SiteAvailability_Span", c => c.Time(nullable: false));
            AddColumn("dbo.ResEvents", "SiteAvailability_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResEvents", "SiteAvailability_Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResEvents", "RegistrationAvailability_Span", c => c.Time(nullable: false));
            AddColumn("dbo.ResEvents", "RegistrationAvailability_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.ResEvents", "RegistrationAvailability_Start", c => c.DateTime(nullable: false));
            DropColumn("dbo.Media", "MediaType");
            DropColumn("dbo.Users", "OperationRole");
            DropColumn("dbo.Users", "AccountStatus");
            DropColumn("dbo.ResEvents", "RegistrationEnd");
            DropColumn("dbo.ResEvents", "RegistrationStart");
        }
    }
}
