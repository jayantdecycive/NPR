namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class additionalinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "AdditionalInformationString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tickets", "AdditionalInformationString");
        }
    }
}
