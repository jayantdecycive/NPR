namespace cfares.entity.dbcontext.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ResUserExtensionsMergeLatLon : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirector_UserId", newName: "UnitMarketingDirectorId");
            AddColumn("dbo.Stores", "Lat", c => c.Double(nullable: false));
            AddColumn("dbo.Stores", "Lon", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "Lon");
            DropColumn("dbo.Stores", "Lat");
            RenameColumn(table: "dbo.Stores", name: "UnitMarketingDirectorId", newName: "UnitMarketingDirector_UserId");
        }
    }
}
