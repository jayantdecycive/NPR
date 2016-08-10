namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enum_cleanup : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirector_UserId", newName: "UnitMarketingDirectorId");
            AddColumn("dbo.Stores", "ConceptCode", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "Lat", c => c.Double(nullable: false));
            AddColumn("dbo.Stores", "Lon", c => c.Double(nullable: false));
            AddColumn("dbo.Stores", "LocationCode", c => c.Int(nullable: false));
            Sql("UPDATE Stores SET LocationCode=LocationCodeId,ConceptCode=ConceptCodeId");
            
            DropColumn("dbo.Stores", "ConceptCodeId");
            DropColumn("dbo.Stores", "LocationCodeId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Stores", "LocationCodeId", c => c.Int(nullable: false));
            AddColumn("dbo.Stores", "ConceptCodeId", c => c.Int(nullable: false));
            Sql("UPDATE Stores SET LocationCodeId=LocationCode,ConceptCodeId=ConceptCode");
            DropColumn("dbo.Stores", "LocationCode");
            DropColumn("dbo.Stores", "Lon");
            DropColumn("dbo.Stores", "Lat");
            DropColumn("dbo.Stores", "ConceptCode");
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirectorId", newName: "UnitMarketingDirector_UserId");
        }
    }
}
