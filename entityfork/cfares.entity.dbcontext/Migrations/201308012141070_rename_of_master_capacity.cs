namespace cfares.entity.dbcontext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rename_of_master_capacity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResEvents", "MaximumCapacity", c => c.Int());
            DropColumn("dbo.ResEvents", "MasterCapacity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResEvents", "MasterCapacity", c => c.Int());
            DropColumn("dbo.ResEvents", "MaximumCapacity");
        }
    }
}
