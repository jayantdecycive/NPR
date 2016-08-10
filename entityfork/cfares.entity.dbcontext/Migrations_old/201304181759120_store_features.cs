namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class store_features : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stores", "AcceptsCfaCard", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasDiningRoom", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "HasDriveThru", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "OffersOnlineOrdering", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "OffersWireless", c => c.Boolean(nullable: false));
            AddColumn("dbo.Stores", "ServesBreakfast", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stores", "ServesBreakfast");
            DropColumn("dbo.Stores", "OffersWireless");
            DropColumn("dbo.Stores", "OffersOnlineOrdering");
            DropColumn("dbo.Stores", "HasDriveThru");
            DropColumn("dbo.Stores", "HasDiningRoom");
            DropColumn("dbo.Stores", "AcceptsCfaCard");
        }
    }
}
