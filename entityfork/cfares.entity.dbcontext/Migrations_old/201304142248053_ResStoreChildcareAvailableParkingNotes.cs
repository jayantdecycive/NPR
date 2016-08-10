namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResStoreChildcareAvailableParkingNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "ChildcareAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "ParkingNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "ParkingNotes");
            DropColumn("dbo.Stores", "ChildcareAvailable");
        }
    }
}
