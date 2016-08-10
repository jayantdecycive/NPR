namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResEvent_MasterCapacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "MasterCapacity", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ResEvents", "MasterCapacity");
        }
    }
}
