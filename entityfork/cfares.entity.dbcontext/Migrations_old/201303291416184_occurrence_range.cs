namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class occurrence_range : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users");
            DropIndex("dbo.Stores", new[] { "FinancialConsultantId" });
            AlterColumn("dbo.Stores", "FinancialConsultantId", c => c.Int());
            AddForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users", "UserId");
            CreateIndex("dbo.Stores", "FinancialConsultantId");
            DropColumn("dbo.Occurrences", "RegistrationAvailability_Start");
            DropColumn("dbo.Occurrences", "RegistrationAvailability_End");
            DropColumn("dbo.Occurrences", "RegistrationAvailability_Span");
            DropColumn("dbo.Occurrences", "SlotRange_Start");
            DropColumn("dbo.Occurrences", "SlotRange_End");
            DropColumn("dbo.Occurrences", "SlotRange_Span");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Occurrences", "SlotRange_Span", c => c.Time(nullable: false));
            AddColumn("dbo.Occurrences", "SlotRange_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Occurrences", "SlotRange_Start", c => c.DateTime(nullable: false));
            AddColumn("dbo.Occurrences", "RegistrationAvailability_Span", c => c.Time(nullable: false));
            AddColumn("dbo.Occurrences", "RegistrationAvailability_End", c => c.DateTime(nullable: false));
            AddColumn("dbo.Occurrences", "RegistrationAvailability_Start", c => c.DateTime(nullable: false));
            DropIndex("dbo.Stores", new[] { "FinancialConsultantId" });
            DropForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users");
            AlterColumn("dbo.Stores", "FinancialConsultantId", c => c.Int(nullable: false));
            CreateIndex("dbo.Stores", "FinancialConsultantId");
            AddForeignKey("dbo.Stores", "FinancialConsultantId", "dbo.Users", "UserId", cascadeDelete: true);
        }
    }
}
