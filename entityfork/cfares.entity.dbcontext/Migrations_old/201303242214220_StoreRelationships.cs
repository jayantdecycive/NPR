namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoreRelationships : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Stores", name: "Distributor_DistributorId", newName: "DistributorId");
            RenameColumn(table: "dbo.Stores", name: "LocationContact_UserId", newName: "LocationContactId");
            RenameColumn(table: "dbo.Stores", name: "MarketingConsultant_UserId", newName: "MarketingConsultantId");
            RenameColumn(table: "dbo.Stores", name: "Operator_UserId", newName: "OperatorId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.Stores", name: "OperatorId", newName: "Operator_UserId");
            RenameColumn(table: "dbo.Stores", name: "MarketingConsultantId", newName: "MarketingConsultant_UserId");
            RenameColumn(table: "dbo.Stores", name: "LocationContactId", newName: "LocationContact_UserId");
            RenameColumn(table: "dbo.Stores", name: "DistributorId", newName: "Distributor_DistributorId");
        }
    }
}
