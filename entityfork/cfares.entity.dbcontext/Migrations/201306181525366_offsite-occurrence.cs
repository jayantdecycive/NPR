namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offsiteoccurrence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Occurrences", "OffSiteAddressId", c => c.Int());
            AddColumn("dbo.Occurrences", "OffSiteDescription", c => c.String());
            AddForeignKey("dbo.Occurrences", "OffSiteAddressId", "dbo.Addresses", "AddressId");
            CreateIndex("dbo.Occurrences", "OffSiteAddressId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Occurrences", new[] { "OffSiteAddressId" });
            DropForeignKey("dbo.Occurrences", "OffSiteAddressId", "dbo.Addresses");
            DropColumn("dbo.Occurrences", "OffSiteDescription");
            DropColumn("dbo.Occurrences", "OffSiteAddressId");
        }
    }
}
