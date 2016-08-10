namespace npr.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class media_owner_id_event_visibility : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Media", name: "Owner_UserId", newName: "OwnerId");
            AddColumn("dbo.ResEvents", "Visibility", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "Visibility");
            RenameColumn(table: "dbo.Media", name: "OwnerId", newName: "Owner_UserId");
        }
    }
}
